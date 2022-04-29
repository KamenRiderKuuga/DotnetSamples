using System;
using System.Diagnostics;
using System.IO;
using System.IO.Pipes;
using System.Threading.Tasks;

namespace Samples.ConsoleApp.Tests
{
    public class NamedPipeTest : ITest
    {
        private readonly string _pipeName = "PipeOfConsoleApp";
        private readonly int _targetCount = 10_000_000;

        public void DoTest()
        {
            StartServer();
            string data = new string('a', 128);

            using (var client = new NamedPipeClientStream(".", _pipeName, PipeDirection.Out))
            using (var writer = new StreamWriter(client))
            {
                client.Connect();

                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Restart();
                for (int i = 0; i < _targetCount; i++)
                {
                    writer.WriteLine(data);
                }

                Console.WriteLine(stopwatch.ElapsedMilliseconds);
            }
        }

        void StartServer()
        {
            Task.Run(() =>
            {
                var server = new NamedPipeServerStream(_pipeName, PipeDirection.In);
                server.WaitForConnection();
                StreamReader reader = new StreamReader(server);
                while (true)
                {
                    try
                    {
                        var line = reader.ReadLine();
                        if (line == null)
                        {
                            throw new IOException("The client disconnected");
                        }
                        // Console.WriteLine("received message：" + line);
                    }
                    catch (IOException)
                    {
                        server.Dispose();
                        reader.Dispose();
                        server = new NamedPipeServerStream(_pipeName, PipeDirection.In);
                        server.WaitForConnection();
                        reader = new StreamReader(server);
                    }
                }
            });
        }
    }
}
