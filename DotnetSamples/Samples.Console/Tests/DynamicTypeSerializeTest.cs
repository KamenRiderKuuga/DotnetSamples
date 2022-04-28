using Samples.Console.Utils;
using System;
using System.Text.Json;

namespace Samples.Console.Tests
{
    internal class DynamicTypeSerializeTest : ITest
    {
        class SampleClass
        {
            public int Field1 { get; set; }

            public string Field2 { get; set; }

            public DateTime Field3 { get; set; }
        }

        public void DoTest()
        {
            int tryTimes = 100000;

            for (int i = 1; i <= 10; i++)
            {
                System.Console.WriteLine("第{0}轮测试，序列化动态类型对象({1}次)，{2}ms", i, tryTimes, TestUtils.TimeMethod(() =>
                {
                    for (int _ = 0; _ < tryTimes; _++)
                    {
                        JsonSerializer.Serialize(new
                        {
                            Field1 = 1,
                            Field2 = "Field2",
                            Field3 = DateTime.Now
                        });
                    }
                }));

                System.Console.WriteLine("第{0}轮测试，序列化类型化对象({1}次)，{2}ms", i, tryTimes, TestUtils.TimeMethod(() =>
                {
                    for (int _ = 0; _ < tryTimes; _++)
                    {
                        JsonSerializer.Serialize(new SampleClass()
                        {
                            Field1 = 1,
                            Field2 = "Field2",
                            Field3 = DateTime.Now
                        });
                    }
                }));
            }
        }
    }
}
