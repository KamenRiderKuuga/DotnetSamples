using System;
using Xunit;
using System.Threading;
using System.Threading.Tasks;

namespace Samples.xUnit
{
    public class TaskTest
    {
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
    }
}
