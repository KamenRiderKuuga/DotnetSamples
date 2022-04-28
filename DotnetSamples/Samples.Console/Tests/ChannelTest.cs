using Samples.Console.Utils;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace Samples.Console.Tests
{
    public class ChannelTest : ITest
    {
        public async void DoTest()
        {
            Task readTask;
            Task writeTask;
            Stopwatch stopwatch = new Stopwatch();

            _ = Task.Run(async () =>
            {
                while (true)
                {
                    await Task.Delay(2000);
                    System.Console.WriteLine($"CPU Usage:{await TestUtils.GetCpuUsageForProcess()}");
                }
            });

            var cts = new CancellationTokenSource();

            for (int i = 1; i <= 2; i++)
            {
                stopwatch.Restart();

                readTask = Task.Run(async () =>
                {
                    await ChannelReadAsync(cts.Token);
                });

                writeTask = Task.Run(() =>
                {
                    ChannelWriteAsync();
                });

                Task.WaitAll(readTask, writeTask);

                System.Console.WriteLine($"使用Channel处理{_targetCount}条数据花费{stopwatch.ElapsedMilliseconds}ms");

                stopwatch.Restart();

                readTask = Task.Run(() =>
                {
                    QueueReadAsync(cts.Token);
                });

                writeTask = Task.Run(() =>
                {
                    QueueWriteAsync();
                });

                Task.WaitAll(readTask, writeTask);

                System.Console.WriteLine($"使用ConcurrentQueue处理{_targetCount}条数据花费{stopwatch.ElapsedMilliseconds}ms");
            }

            System.Console.WriteLine("Channel开始等待数据");
            _ = Task.Run(async () =>
            {
                await ChannelReadAsync(cts.Token);
            });

            await Task.Delay(20000);
            cts.Cancel();
            System.Console.WriteLine("Channel结束等待数据");

            var queueCts = new CancellationTokenSource();

            System.Console.WriteLine("ConcurrentQueue开始等待数据");

            _ = Task.Run(() =>
            {
                QueueReadAsync(queueCts.Token);
            });

            await Task.Delay(20000);

            queueCts.Cancel();

            System.Console.WriteLine("ConcurrentQueue结束等待数据");

        }

        private const int _targetCount = 100_000_000;
        private readonly Channel<User> _channel = Channel.CreateUnbounded<User>();
        private readonly ConcurrentQueue<User> _queue = new ConcurrentQueue<User>();

        class User
        {
            public string Name { get; set; }

            public UserInfo UserInfo { get; set; }
        }

        class UserInfo
        {
            public int Age { get; set; }
        }

        async Task ChannelReadAsync(CancellationToken token = default)
        {
            ChannelReader<User> reader = _channel.Reader;
            var totalCount = 0;

            while (!token.IsCancellationRequested)
            {
                var user = await reader.ReadAsync(token);
                totalCount += 1;
                if (totalCount == _targetCount)
                {
                    break;
                }
            }
        }

        void ChannelWriteAsync()
        {
            ChannelWriter<User> writer = _channel.Writer;
            for (int i = 0; i < _targetCount; i++)
            {
                writer.TryWrite(new User
                {
                    Name = "Channel Name",
                    UserInfo = new UserInfo
                    {
                        Age = i,
                    }
                });
            }
        }

        void QueueReadAsync(CancellationToken token = default)
        {
            var totalCount = 0;
            while (!token.IsCancellationRequested)
            {
                _queue.TryDequeue(out var item);
                if (item != null)
                {
                    totalCount += 1;
                    if (totalCount == _targetCount)
                    {
                        break;
                    }
                }
            }
        }

        void QueueWriteAsync()
        {
            for (int i = 0; i < _targetCount; i++)
            {
                _queue.Enqueue(new User
                {
                    Name = "Channel Name",
                    UserInfo = new UserInfo
                    {
                        Age = i,
                    }
                });
            }
        }
    }
}
