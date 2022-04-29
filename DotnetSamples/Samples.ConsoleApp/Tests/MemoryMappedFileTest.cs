using System;
using System.Diagnostics;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Text;
using System.Threading.Tasks;

namespace Samples.ConsoleApp.Tests
{
    internal class MemoryMappedFileTest : ITest
    {
        private readonly int _targetCount = 10_000_000;
        private readonly string _fileName = "MemoryMappedFile.data";

        public void DoTest()
        {
            using (var fs = new FileStream(_fileName, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                fs.SetLength(128 + 4);
            }

            StartServer();
            byte[] buffer = Encoding.UTF8.GetBytes(new string('a', 128));

            using (FileStream fs = new FileStream(_fileName, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite))
            using (MemoryMappedFile mmf = MemoryMappedFile.CreateFromFile(fs, null, fs.Length, MemoryMappedFileAccess.ReadWrite, HandleInheritability.None, true))
            using (var accessor = mmf.CreateViewAccessor())
            {
                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Restart();

                for (int i = 0; i < _targetCount; i++)
                {
                    accessor.WriteArray(0, buffer, 0, buffer.Length);
                    accessor.Write(128, i);
                }

                Console.WriteLine(stopwatch.ElapsedMilliseconds);
            }
        }

        void StartServer()
        {
            byte[] data = new byte[128];
            byte[] countData = new byte[4];

            Task.Run(() =>
            {
                using (FileStream fs = new FileStream(_fileName, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite))
                using (MemoryMappedFile mmf = MemoryMappedFile.CreateFromFile(fs, null, fs.Length, MemoryMappedFileAccess.ReadWrite, HandleInheritability.None, true))
                using (var accessor = mmf.CreateViewAccessor())
                {
                    while (true)
                    {
                        accessor.ReadArray(0, data, 0, data.Length);
                        var content = Encoding.UTF8.GetString(data);
                        accessor.ReadArray(128, countData, 0, countData.Length);
                        // Console.WriteLine(BitConverter.ToInt32(countData));
                    }
                }
            });
        }
    }
}
