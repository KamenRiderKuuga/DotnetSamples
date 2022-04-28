using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Samples.ConsoleApp.Utils
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

        /// <summary>
        /// Gets the CPU useage of current process
        /// </summary>
        /// <remarks>Code from: https://medium.com/@jackwild/getting-cpu-usage-in-net-core-7ef825831b8b</remarks>
        /// <returns></returns>
        public static async Task<double> GetCpuUsageForProcess()
        {
            var startTime = DateTime.UtcNow;

            var startCpuUsage = Process.GetCurrentProcess().TotalProcessorTime;
            await Task.Delay(500);

            var endTime = DateTime.UtcNow;
            var endCpuUsage = Process.GetCurrentProcess().TotalProcessorTime;
            var cpuUsedMs = (endCpuUsage - startCpuUsage).TotalMilliseconds;
            var totalMsPassed = (endTime - startTime).TotalMilliseconds;
            var cpuUsageTotal = cpuUsedMs / (Environment.ProcessorCount * totalMsPassed);
            return cpuUsageTotal * 100;
        }
    }
}
