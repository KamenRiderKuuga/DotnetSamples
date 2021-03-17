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
            Thread childThread = new Thread(writeThreadName);
            childThread.Start();

            writeThreadName();
        }

        [TestMethod("只有一个前台线程运行")]
        public void OnlyOneForegroundThread()
        {
            Thread childThread = new Thread(writeThreadName);
            childThread.Start();
            // 因为处于testhost，需要等待ForegroundThread执行完毕
            childThread.Join();
        }

        public static void writeThreadName()
        {
            for (int i = 0; i < 10000; i++)
            {
                Console.WriteLine($"{i}", Thread.CurrentThread.ManagedThreadId);
            }
        }
    }
}
