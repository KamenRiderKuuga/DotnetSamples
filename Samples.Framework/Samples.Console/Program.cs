using Samples.Console.Utils;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Unicode;
using System.Xml;

namespace Samples.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            ExcuteTest(Constants.HTML_READER);

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

                case Constants.DYNAMIC_TYPE_SERIALIZE:
                    DynamicTypeSerialize.DynamicTypeSerializeTest();
                    break;

                case Constants.TEXT_JSON_CUSTOM:
                    TextJsonCustom.TextJsonCustomTest();
                    break;

                case Constants.HTML_READER:
                    HTMLReader.HTMLReaderTest();
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

        private static List<string> InitDictionary()
        {
            System.Console.WriteLine("第一次引用List对象，List初始化成功");
            return new List<string>();
        }
    }

    public class DynamicTypeSerialize
    {
        public class SampleClass
        {
            public int Field1 { get; set; }

            public string Field2 { get; set; }

            public DateTime Field3 { get; set; }
        }

        public static void DynamicTypeSerializeTest()
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

    public class TextJsonCustom
    {
        public static void TextJsonCustomTest()
        {
            System.Console.OutputEncoding = System.Text.Encoding.UTF8;

            var serverInfo = new ServerInfo()
            {
                ServerName = "服务器名称",
                ServerID = 1,
                StartTime = DateTime.Now
            };

            var content = JsonSerializer.Serialize(serverInfo, new JsonSerializerOptions()
            {
                Encoder = JavaScriptEncoder.Create(UnicodeRanges.All)
            });

            System.Console.WriteLine(content);

            var contentValue = JsonSerializer.Deserialize<ServerInfo>(content);
        }

        /// <summary>
        /// 服务器信息
        /// </summary>
        public class ServerInfo
        {
            /// <summary>
            /// 服务器名称
            /// </summary>
            [JsonPropertyName("server_name")]
            public string ServerName { get; set; }

            /// <summary>
            /// 服务器ID
            /// </summary>
            [JsonPropertyName("server_id")]
            public int ServerID { get; set; }

            /// <summary>
            /// start_time
            /// </summary>
            [JsonConverter(typeof(CustomDateTimeConverter))]
            [JsonPropertyName("start_time")]
            public DateTimeOffset StartTime { get; set; }
        }

        public class CustomDateTimeConverter : JsonConverter<DateTimeOffset>
        {
            public override DateTimeOffset Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                var content = reader.GetString();

                if (long.TryParse(content, out var result))
                {
                    return DateTimeOffset.FromUnixTimeSeconds(result);
                }
                else
                {
                    return new DateTimeOffset();
                }
            }

            public override void Write(Utf8JsonWriter writer, DateTimeOffset value, JsonSerializerOptions options)
            {
                writer.WriteStringValue(value.ToUnixTimeSeconds().ToString());
            }
        }
    }

    public class HTMLReader
    {
        public static void HTMLReaderTest()
        {
            var content = @"<p><strong><span style=""font-size: 12pt;"">各位亲爱的玩家：</span></strong></p><p><span style=""font-size: 12pt;"">&nbsp;&nbsp; &nbsp;&nbsp;&nbsp;非常感谢您对<span style=""color: #ff9900;"">《逍遥情缘》</span>的关注和热情。为了想能更好的体验游戏，我们正在对服务进行最后一-轮优化。服务器将推迟开放，为此我们深感歉意。我们会尽快解决问题，开服后，我们会及时奉上开服大礼包。期待您届时再次登录查收。&lt;p&gt;p123&lt;/p&gt;</span></p><p style=""text-align: right;""><strong><span style=""font-size: 12pt;"">官网运营团队</span></strong></p>";
            
            content = @$"<!DOCTYPE documentElement[
                      <!ENTITY Alpha ""&#913;"">
                      <!ENTITY ndash ""&#8211;"">
                      <!ENTITY mdash ""&#8212;"">
                      <!ENTITY nbsp ""&#160;"">
                      ]><root>{content}</root>";


            XmlReaderSettings settings = new XmlReaderSettings()
            {
                DtdProcessing = DtdProcessing.Parse
            };

            using (XmlReader reader = XmlReader.Create(new StringReader(content), settings))
            {
                while (reader.Read())
                {
                    switch (reader.NodeType)
                    {
                        case XmlNodeType.Element:
                            System.Diagnostics.Debug.WriteLine("Start Element {0}", reader.Name);
                            break;
                        case XmlNodeType.Text:
                            System.Diagnostics.Debug.WriteLine("Text Node: {0}", reader.Value);
                            break;
                        case XmlNodeType.EndElement:
                            System.Diagnostics.Debug.WriteLine("End Element {0}", reader.Name);
                            break;
                        default:
                            // do nothing
                            break;
                    }
                }
            }
        }
    }

    public class Constants
    {
        public const string ENUMS_DESCRIPTION_TEST = "获取枚举信息测试，对比反射速度以及字典速度";

        public const string STATIC_FIELD_TEST = "静态变量测试，主要是为了测试其初始化时机，初始化的线程安全性，容量上限";

        public const string DYNAMIC_TYPE_SERIALIZE = "动态类型序列化效率测试";

        public const string TEXT_JSON_CUSTOM = "System.Text.Json自定义序列化/反序列化测试";

        public const string HTML_READER = "测试使用XmlReader对HTML进行解析及转换";
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
