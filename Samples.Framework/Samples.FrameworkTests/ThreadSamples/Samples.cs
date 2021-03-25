using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading;

namespace Samples.FrameworkTests.ThreadSamples
{
    [TestClass]
    public class Samples
    {
        [TestMethod("主线程和子线程同时运行")]
        public void MainThreadRunWithChild()
        {
            Thread childThread = new Thread(WriteThreadName);
            childThread.Start();

            WriteThreadName();
        }

        [TestMethod("只有一个前台线程运行")]
        public void OnlyOneForegroundThread()
        {
            Thread childThread = new Thread(WriteThreadName);
            childThread.Start();
            // 因为处于当前线程处于Background线程，使用Join等待ForegroundThread执行完毕
            childThread.Join();
        }

        [TestMethod("指定等待线程结束的时间")]
        public void WaitThreadJoinWithTime()
        {
            Thread childThread = new Thread(() =>
            {
                Thread.Sleep(5000);
                WriteThreadName();
            });

            childThread.Start();
            Console.WriteLine(childThread.Join(3000));
            Console.WriteLine(childThread.Join(3000));
        }


        public static void WriteThreadName()
        {
            for (int i = 0; i < 10000; i++)
            {
                Console.WriteLine($"{i}", Thread.CurrentThread.ManagedThreadId);
            }
        }
    }
}
