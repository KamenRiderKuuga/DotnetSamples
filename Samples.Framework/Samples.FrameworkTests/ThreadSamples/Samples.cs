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

        [TestMethod("交集测试")]
        public void CaculateTimesBetweenToTimeRange()
        {
            var firstTwoTimes = (new DateTime(2012, 11, 12), new DateTime(2013, 12, 13));
            var secondTwoTimes = (new DateTime(2012, 12, 12), new DateTime(2011, 11, 13));

            GetOverlappingTimeRange(firstTwoTimes, secondTwoTimes);
        }

        public static (DateTime, DateTime)? GetOverlappingTimeRange((DateTime, DateTime) firstTimeRange,
                                                                    (DateTime, DateTime) secondTimeRange)
        {
            var leftIndex = Math.Max(firstTimeRange.Item1.Ticks, secondTimeRange.Item1.Ticks);
            var rightIndex = Math.Min(firstTimeRange.Item2.Ticks, secondTimeRange.Item2.Ticks);

            if (leftIndex > rightIndex)
            {
                return null;
            }
            else
            {
                return (new DateTime(leftIndex), new DateTime(rightIndex));
            }
        }

    }
}
