using System;
using Xunit;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using Xunit.Abstractions;

namespace Samples.xUnit
{
    public class TaskTest
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public TaskTest(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }

        [Fact]
        public void LongRunnigTaskTest()
        {
            // 创建一个长时间运行的Task，这将不使用线程池中的线程，而是启动一个线程池之外的额外线程
            var longRunningTask = Task.Factory.StartNew(_ =>
            {
                Thread.Sleep(3000);
                Console.WriteLine("Foo");
            }, TaskCreationOptions.LongRunning);
            longRunningTask.Wait();
        }


        /// <summary>
        /// 测试多线程时，由于自增不能保证原子性，造成最后的数值没有达到预期
        /// </summary>
        [Fact]
        public void RunTaskToAdd()
        {
            var tasks = new List<Task>();
            int currentValue = 0;

            // 使用Interlocked.Increment自增
            for (int i = 0; i < 1000; i++)
            {
                tasks.Add(new Task(() =>
                {
                    for (int i = 0; i < 1000; i++)
                    {
                        Interlocked.Increment(ref currentValue);
                    }
                }));
            }

            tasks.ForEach(item => item.Start());
            Task.WaitAll(tasks.ToArray());
            Console.WriteLine($"最后自增结果是{currentValue}");

            currentValue = 0;
            tasks.Clear();

            // 使用i++自增
            for (int i = 0; i < 1000; i++)
            {
                tasks.Add(new Task(() =>
                {
                    for (int i = 0; i < 1000; i++)
                    {
                        currentValue++;
                    }
                }));
            }

            tasks.ForEach(item => item.Start());
            Task.WaitAll(tasks.ToArray());
            Console.WriteLine($"最后自增结果是{currentValue}");
        }

        /// <summary>
        /// 两个线程交替输出单双数
        /// </summary>
        [Fact]
        public void TwoThreadPrint()
        {
            AutoResetEvent eventForSingle = new AutoResetEvent(true);
            AutoResetEvent eventForDouble = new AutoResetEvent(false);

            var number = 0;

            Thread threadForSingle = new Thread(() =>
            {
                while (number < 999)
                {
                    eventForSingle.WaitOne();
                    number++;
                    _testOutputHelper.WriteLine(number.ToString());
                    eventForDouble.Set();
                }
            });

            Thread threadForDouble = new Thread(() =>
            {
                while (number < 999)
                {
                    eventForDouble.WaitOne();
                    number++;
                    _testOutputHelper.WriteLine(number.ToString());
                    eventForSingle.Set();
                }
            });

            threadForSingle.Start();
            threadForDouble.Start();

            Thread.Sleep(10000);

            eventForSingle.Dispose();
            eventForDouble.Dispose();
        }
    }
}
