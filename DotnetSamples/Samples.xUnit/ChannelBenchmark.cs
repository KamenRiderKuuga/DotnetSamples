using System.Collections.Concurrent;
using System.Diagnostics;
using System.Threading.Channels;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace Samples.xUnit
{
    public class ChannelBenchmark
    {
        private const int _targetCount = 100_000_000;
        private readonly Channel<User> _channel = Channel.CreateUnbounded<User>();

        private readonly ConcurrentQueue<User> _queue = new ConcurrentQueue<User>();

        private readonly ITestOutputHelper _testOutputHelper;

        public ChannelBenchmark(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }

        [Fact(DisplayName = "compare Channel with ConcurrentQueue")]
        public void ChannelBenchmarkTest()
        {
            Task readTask;
            Task writeTask;
            Stopwatch stopwatch = new Stopwatch();

            for (int i = 1; i <= 2; i++)
            {
                stopwatch.Restart();

                readTask = Task.Run(async () =>
                {
                    await ChannelReadAsync();
                });

                writeTask = Task.Run(() =>
                {
                    ChannelWriteAsync();
                });

                Task.WaitAll(readTask, writeTask);

                _testOutputHelper.WriteLine($"使用Channel处理{_targetCount}条数据花费{stopwatch.ElapsedMilliseconds}ms");

                stopwatch.Restart();

                readTask = Task.Run(() =>
                {
                    QueueReadAsync();
                });

                writeTask = Task.Run(() =>
                {
                    QueueWriteAsync();
                });

                Task.WaitAll(readTask, writeTask);

                _testOutputHelper.WriteLine($"使用ConcurrentQueue处理{_targetCount}条数据花费{stopwatch.ElapsedMilliseconds}ms");
            }
        }

        class User
        {
            public string Name { get; set; }

            public UserInfo UserInfo { get; set; }
        }

        class UserInfo
        {
            public int Age { get; set; }
        }

        async Task ChannelReadAsync()
        {
            ChannelReader<User> reader = _channel.Reader;
            var totalCount = 0;

            while (true)
            {
                var user = await reader.ReadAsync();
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

        void QueueReadAsync()
        {
            var totalCount = 0;
            while (true)
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
