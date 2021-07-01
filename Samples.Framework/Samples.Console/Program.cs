using Samples.Console.Utils;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;

namespace Samples.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            ExcuteTest(Constants.STATIC_FIELD_TEST);

            System.Console.ReadKey();
        }

        private static void ExcuteTest(string testFlag)
        {
            switch (testFlag)
            {
                case Constants.ENUMS_DESCRIPTION_TEST:
                    EnumsDescription.EnumsDescriptionTest();
                    break;

                case Constants.STATIC_FIELD_TEST:
                    StaticField.StaticFieldTest();
                    break;

                default:
                    break;
            }
        }
    }

    public class EnumsDescription
    {
        private static Dictionary<Enum, string> _dic = new Dictionary<Enum, string>();
        private static Dictionary<Enum, string> _dicReadOnly = new Dictionary<Enum, string>()
        {
            { SampleEnums.First,SampleEnums.First.GetRemark()},
            { SampleEnums.Second,SampleEnums.Second.GetRemark()},
            { SampleEnums.Third,SampleEnums.Third.GetRemark()}
        };

        private static ConcurrentDictionary<Enum, string> _concurrentDic = new ConcurrentDictionary<Enum, string>();
        private static ConcurrentDictionary<Enum, string> _concurrentDicReadOnly = new ConcurrentDictionary<Enum, string>();

        public static void EnumsDescriptionTest()
        {
            _concurrentDicReadOnly.TryAdd(SampleEnums.First, SampleEnums.First.GetRemark());
            _concurrentDicReadOnly.TryAdd(SampleEnums.Second, SampleEnums.Second.GetRemark());
            _concurrentDicReadOnly.TryAdd(SampleEnums.Third, SampleEnums.Third.GetRemark());

            System.Diagnostics.Stopwatch watch = new System.Diagnostics.Stopwatch();

            int tryTimes = 100000;

            // 由于预编译的原因，这里测试两次
            for (int i = 1; i < 3; i++)
            {
                System.Console.WriteLine("第{0}轮测试，直接反射花费的时间({1}次)，{2}ms", i, tryTimes, TestUtils.TimeMethod(() =>
                {
                    for (int _ = 0; _ < tryTimes; _++)
                    {
                        SampleEnums.First.GetRemark();
                        SampleEnums.Second.GetRemark();
                        SampleEnums.Third.GetRemark();
                    }
                }));


                System.Console.WriteLine("第{0}轮测试，使用字典做缓存花费的时间({1}次)，{2}ms", i, tryTimes, TestUtils.TimeMethod(() =>
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

                System.Console.WriteLine("第{0}轮测试，使用线程安全的字典做缓存花费的时间({1}次)，{2}ms", i, tryTimes, TestUtils.TimeMethod(() =>
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

                System.Console.WriteLine("第{0}轮测试，使用预初始化好的字典做缓存花费的时间({1}次)，{2}ms", i, tryTimes, TestUtils.TimeMethod(() =>
                {
                    for (int _ = 0; _ < tryTimes; _++)
                    {
                        _dicReadOnly.TryGetValue(SampleEnums.First, out var _);
                        _dicReadOnly.TryGetValue(SampleEnums.Second, out var _);
                        _dicReadOnly.TryGetValue(SampleEnums.Third, out var _);
                    }
                }));

                System.Console.WriteLine("第{0}轮测试，使用预初始化好的线程安全的字典做缓存花费的时间({1}次)，{2}ms", i, tryTimes, TestUtils.TimeMethod(() =>
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

    public class StaticField
    {
        private static readonly List<string> _dic = InitDictionary();

        public static void StaticFieldTest()
        {
            System.Console.WriteLine("程序开始运行");

            _dic.Add(string.Empty);
            _dic.Add(string.Empty);
        }

        public static List<string> InitDictionary()
        {
            System.Console.WriteLine("第一次引用List对象，List初始化成功");
            return new List<string>();
        }
    }

    public class Constants
    {
        public const string ENUMS_DESCRIPTION_TEST = "获取枚举信息测试，对比反射速度以及字典速度";

        public const string STATIC_FIELD_TEST = "静态变量测试，主要是为了测试其初始化时机，初始化的线程安全性，容量上限";
    }

    public enum SampleEnums
    {
        [Description("第一个枚举")]
        First = 1,

        [Description("第二个枚举")]
        Second = 2,

        [Description("第三个枚举")]
        Third = 3
    }
}
