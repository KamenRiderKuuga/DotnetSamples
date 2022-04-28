using Samples.ConsoleApp.Utils;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;

namespace Samples.ConsoleApp.Tests
{
    public class EnumsDescriptionTest : ITest
    {
        private readonly Dictionary<Enum, string> _dic = new Dictionary<Enum, string>();
        private readonly Dictionary<Enum, string> _dicReadOnly = new Dictionary<Enum, string>()
        {
            { SampleEnums.First,SampleEnums.First.GetRemark()},
            { SampleEnums.Second,SampleEnums.Second.GetRemark()},
            { SampleEnums.Third,SampleEnums.Third.GetRemark()}
        };

        enum SampleEnums
        {
            [Description("第一个枚举")]
            First = 1,

            [Description("第二个枚举")]
            Second = 2,

            [Description("第三个枚举")]
            Third = 3
        }

        private readonly ConcurrentDictionary<Enum, string> _concurrentDic = new ConcurrentDictionary<Enum, string>();
        private readonly ConcurrentDictionary<Enum, string> _concurrentDicReadOnly = new ConcurrentDictionary<Enum, string>();

        public void DoTest()
        {
            _concurrentDicReadOnly.TryAdd(SampleEnums.First, SampleEnums.First.GetRemark());
            _concurrentDicReadOnly.TryAdd(SampleEnums.Second, SampleEnums.Second.GetRemark());
            _concurrentDicReadOnly.TryAdd(SampleEnums.Third, SampleEnums.Third.GetRemark());

            Stopwatch watch = new Stopwatch();

            int tryTimes = 100000;

            // 由于预编译的原因，这里测试两次
            for (int i = 1; i < 3; i++)
            {
                Console.WriteLine("第{0}轮测试，直接反射花费的时间({1}次)，{2}ms", i, tryTimes, TestUtils.TimeMethod(() =>
                {
                    for (int _ = 0; _ < tryTimes; _++)
                    {
                        SampleEnums.First.GetRemark();
                        SampleEnums.Second.GetRemark();
                        SampleEnums.Third.GetRemark();
                    }
                }));

                Console.WriteLine("第{0}轮测试，使用字典做缓存花费的时间({1}次)，{2}ms", i, tryTimes, TestUtils.TimeMethod(() =>
                {
                    for (int _ = 0; _ < tryTimes; _++)
                    {
                        if (!_dic.TryGetValue(SampleEnums.First, out var _))
                        {
                            _dic[SampleEnums.First] = SampleEnums.First.GetRemark();
                        };
                        if (!_dic.TryGetValue(SampleEnums.Second, out var _))
                        {
                            _dic[SampleEnums.Second] = SampleEnums.Second.GetRemark();
                        };
                        if (!_dic.TryGetValue(SampleEnums.Third, out var _))
                        {
                            _dic[SampleEnums.Third] = SampleEnums.Third.GetRemark();
                        };
                    }
                }));

                Console.WriteLine("第{0}轮测试，使用线程安全的字典做缓存花费的时间({1}次)，{2}ms", i, tryTimes, TestUtils.TimeMethod(() =>
                {
                    for (int _ = 0; _ < tryTimes; _++)
                    {
                        if (!_concurrentDic.TryGetValue(SampleEnums.First, out var _))
                        {
                            _concurrentDic[SampleEnums.First] = SampleEnums.First.GetRemark();
                        };
                        if (!_concurrentDic.TryGetValue(SampleEnums.Second, out var _))
                        {
                            _concurrentDic[SampleEnums.Second] = SampleEnums.Second.GetRemark();
                        };
                        if (!_concurrentDic.TryGetValue(SampleEnums.Third, out var _))
                        {
                            _concurrentDic[SampleEnums.Third] = SampleEnums.Third.GetRemark();
                        };
                    }
                }));

                Console.WriteLine("第{0}轮测试，使用预初始化好的字典做缓存花费的时间({1}次)，{2}ms", i, tryTimes, TestUtils.TimeMethod(() =>
                {
                    for (int _ = 0; _ < tryTimes; _++)
                    {
                        _dicReadOnly.TryGetValue(SampleEnums.First, out var _);
                        _dicReadOnly.TryGetValue(SampleEnums.Second, out var _);
                        _dicReadOnly.TryGetValue(SampleEnums.Third, out var _);
                    }
                }));

                Console.WriteLine("第{0}轮测试，使用预初始化好的线程安全的字典做缓存花费的时间({1}次)，{2}ms", i, tryTimes, TestUtils.TimeMethod(() =>
                {
                    for (int _ = 0; _ < tryTimes; _++)
                    {
                        _concurrentDicReadOnly.TryGetValue(SampleEnums.First, out var _);
                        _concurrentDicReadOnly.TryGetValue(SampleEnums.Second, out var _);
                        _concurrentDicReadOnly.TryGetValue(SampleEnums.Third, out var _);
                    }
                }));
            }
        }
    }
}
