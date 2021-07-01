using System;
using System.Diagnostics;

namespace Samples.Console.Utils
{
    public class TestUtils
    {
        public static long TimeMethod(Action methodToTime)
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            methodToTime();
            stopwatch.Stop();
            return stopwatch.ElapsedMilliseconds;
        }
    }
}
