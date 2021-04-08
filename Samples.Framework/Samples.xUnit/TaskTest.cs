using System;
using Xunit;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Samples.xUnit
{
    public class TaskTest
    {
        // [Fact]
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
    }
}
