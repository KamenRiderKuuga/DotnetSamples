using System;
using System.Collections.Generic;
using System.Threading;

namespace Samples.Framework.QueueHandler
{
    /// <summary>
    /// 队列处理基类，用于定义处理队列数据的主流程
    /// </summary>
    /// <typeparam name="T">待处理的队列中数据的实体类型</typeparam>
    public abstract class HandlerBase<T>
    {
        private bool _running;
        private int _threadCount;
        private List<Thread> _threads;

        /// <summary>
        /// 最大线程数
        /// </summary>
        private const int MAX_THREAD_COUNT = 50;

        #region 需要根据具体情况重写的抽象属性

        /// <summary>
        /// 队列名，即数据的来源队列
        /// </summary>
        public abstract string QueueName { get; }

        /// <summary>
        /// 临时队列后缀，用于避免启动多个服务时出现临时队列同名的情况
        /// </summary>
        public abstract string Suffix { get; }

        #endregion

        public HandlerBase(int threadCount = 1)
        {
            _threadCount = threadCount;
        }

        #region 根据具体情况需要进行重写的抽象函数

        /// <summary>
        /// 将数据从来源临时队列转移到目标临时队列
        /// </summary>
        /// <param name="fromQueue">来源队列名</param>
        /// <param name="toQueue">转移到的队列名</param>
        /// <remarks>用于确保之前的队列没有残留的未处理数据</remarks>
        /// <returns></returns>
        public abstract T TransferNext(string fromQueue, string toQueue);

        /// <summary>
        /// 从主队列获取下一条要处理的数据，并且把这条数据放在指定临时队列
        /// </summary>
        /// <param name="tempQueue">临时队列名</param>
        /// <returns></returns>
        public abstract T GetNext(string tempQueue);

        /// <summary>
        /// 根据实体返回其主键
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public abstract string GetPrimaryKey(T entity);

        /// <summary>
        /// 获取指定实体的重试信息，如果没有找到，认为其没有执行过
        /// </summary>
        /// <param name="field"></param>
        /// <returns></returns>
        public abstract RetryInfo GetRetryInfo(string field);

        /// <summary>
        /// 执行处理流程
        /// </summary>
        /// <param name="entity">用于执行的实体</param>
        /// <returns></returns>
        public abstract bool ExecuteHandle(T entity);

        /// <summary>
        /// 执行成功后要执行的函数
        /// </summary>
        /// <param name="entity">当前处理的实体</param>
        /// <param name="tempQueueName">当前数据所处的临时队列的名称</param>
        /// <param name="retryInfo">重试任务所需的相关信息</param>
        /// <returns></returns>
        public abstract void ExecuteSuccess(T entity, string tempQueueName, RetryInfo retryInfo);

        /// <summary>
        /// 执行失败后要进行的函数
        /// </summary>
        /// <param name="entity">当前处理的实体</param>
        /// <param name="tempQueueName">当前数据所处的临时队列的名称</param>
        /// <param name="retryInfo">重试任务所需的相关信息</param>
        /// <returns></returns>
        public abstract void ExecuteFail(T entity, string tempQueueName, RetryInfo retryInfo);

        #endregion

        #region 私有函数

        /// <summary>
        /// 获得临时队列名，同时这也将作为线程的名字
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        private string GetQueueName(int num)
        {
            return $"{QueueName}_Temp_{Suffix}_{num}";
        }

        #endregion

        #region 任务处理主流程

        /// <summary>
        /// 开始进行处理
        /// </summary>
        public virtual void StartHandle()
        {
            _running = true;
            _threads = new List<Thread>();

            // 线程数需要在指定范围内
            if ((_threadCount < 0) || (_threadCount > MAX_THREAD_COUNT))
            {
                _threadCount = 1;
            }

            for (int num = 0; num < _threadCount; num++)
            {
                Thread thread = new Thread(HandleData);
                thread.Name = GetQueueName(num);
                thread.IsBackground = true;
                thread.Start();
                _threads.Add(thread);
            }
        }

        /// <summary>
        /// 判断任务是否已经到可以执行的时间，并返回相关的重试信息
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="retryInfo"></param>
        /// <returns></returns>
        public virtual bool CanExecute(T entity, out RetryInfo retryInfo)
        {
            string primaryKey = GetPrimaryKey(entity);

            if (!string.IsNullOrWhiteSpace(primaryKey))
            {
                retryInfo = GetRetryInfo(primaryKey);

                // 还没到可以执行的时间
                if (DateTime.Now >= retryInfo.RetryTime)
                {
                    return true;
                }
            }
            else
            {
                retryInfo = null;
            }

            return false;
        }

        /// <summary>
        /// 各线程要执行的任务，从队列中不断获取数据，并进行处理
        /// </summary>
        public virtual void HandleData()
        {
            string tempQueueName = Thread.CurrentThread.Name;

            // 如果当前处于一号线程，且开启的线程数没有达到上限，从大于本次设置线程数的队列转移所有数据
            if (tempQueueName == GetQueueName(0))
            {
                for (int number = _threadCount + 1; number <= MAX_THREAD_COUNT; number++)
                {
                    var entity = TransferNext(GetQueueName(number), tempQueueName);

                    while (entity != null)
                    {
                        TransferNext(GetQueueName(number), tempQueueName);
                    }
                }
            }

            do
            {
                var nextEntity = GetNext(tempQueueName);

                // 如果任何数据都获取不到，Sleep一秒，减少访问频率
                if (nextEntity == null)
                {
                    Thread.Sleep(1000);
                }
                else
                {
                    if (CanExecute(nextEntity, out var retryInfo))
                    {
                        if (ExecuteHandle(nextEntity))
                        {
                            ExecuteSuccess(nextEntity, tempQueueName, retryInfo);
                        }
                        else
                        {
                            ExecuteFail(nextEntity, tempQueueName, retryInfo);
                        }
                    }
                }

            } while (_running);
        }

        /// <summary>
        /// 停止各个线程的任务处理
        /// </summary>
        public virtual void StopHandle()
        {
            _running = false;
            // 等待所有线程都执行完当前的任务再退出，防止因为停止时因为正在处理数据发生意料之外的问题
            foreach (var thread in _threads)
            {
                while (thread?.IsAlive == true)
                {
                    // do nothing
                }
            }
        }

        #endregion
    }
}
