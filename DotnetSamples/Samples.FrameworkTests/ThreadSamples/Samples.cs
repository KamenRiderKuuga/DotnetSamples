using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading;
using System.Threading.Tasks;

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

        [TestMethod("启动新线程，并传入参数")]
        public void WriteSomething()
        {
            Thread thread = new Thread(() => WriteSomething("Hello World!"));
            thread.Start();
            thread.Join();
        }

        [TestMethod("测试线程信号")]
        public void SignalTest()
        {
            var signal = new ManualResetEvent(false);
            var thread = new Thread(() =>
           {
               Console.WriteLine("Waiting for signal");
               signal.WaitOne();
               signal.Dispose();
               Console.WriteLine("Got signal!");
           });

            thread.Start();

            Thread.Sleep(3000);
            signal.Set(); // 打开信号
            thread.Join();
        }

        [TestMethod("开始一个Task")]
        public void RunTask()
        {
            Task task = Task.Run(() =>
            {
                Thread.Sleep(3000);
                Console.WriteLine("Foo");
            });
            Console.WriteLine(task.IsCompleted);
            task.Wait();
        }

        public static void WriteSomething(string message)
        {
            Console.WriteLine(message);
        }

        public static void WriteThreadName()
        {
            for (int i = 0; i < 10000; i++)
            {
                Console.WriteLine($"{i}", Thread.CurrentThread.ManagedThreadId);
            }
        }

        public static ThreadState SimpleThreadState(ThreadState threadState)
        {
            return threadState & (ThreadState.Unstarted | ThreadState.WaitSleepJoin | ThreadState.Stopped);
        }
    }
}
