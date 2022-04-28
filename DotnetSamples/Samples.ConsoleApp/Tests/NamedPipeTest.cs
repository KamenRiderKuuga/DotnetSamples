using System;
using System.IO;
using System.IO.Pipes;
using System.Threading.Tasks;

namespace Samples.ConsoleApp.Tests
{
    public class NamedPipeTest : ITest
    {
        private readonly string _pipeName = "PipeOfConsoleApp";

        public void DoTest()
        {
            StartServer();

            using (var client = new NamedPipeClientStream(".", _pipeName, PipeDirection.Out))
            {
                client.Connect();
                StreamWriter writer = new StreamWriter(client);

                while (true)
                {
                    string input = Console.ReadLine();
                    if (string.IsNullOrEmpty(input)) break;
                    writer.WriteLine(input);
                    writer.Flush();
                }
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
                        Console.WriteLine("received message：" + line);
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
