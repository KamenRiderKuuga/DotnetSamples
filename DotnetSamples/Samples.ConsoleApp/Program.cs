using Samples.ConsoleApp.Tests;
using System;

namespace Samples.ConsoleApp
{
    class Program
    {
        static void Main()
        {
            ExcuteTest(Constants.DICTIONARY_TEST);
            Console.ReadKey();
        }

        private static void ExcuteTest(string testFlag)
        {
            ITest test;

            test = testFlag switch
            {
                Constants.ENUMS_DESCRIPTION_TEST => new EnumsDescriptionTest(),
                Constants.STATIC_FIELD_TEST => new StaticFieldTest(),
                Constants.DYNAMIC_TYPE_SERIALIZE => new DynamicTypeSerializeTest(),
                Constants.TEXT_JSON_CUSTOM => new TextJsonCustomTest(),
                Constants.HTML_READER => new HTMLReaderTest(),
                Constants.REFLECTION_CACHE => new ReflectionCacheTest(),
                Constants.CHANNEL_TEST => new ChannelTest(),
                Constants.NAMEDPIPE_TEST => new NamedPipeTest(),
                Constants.MEMORYMAPPEDFILE_TEST => new MemoryMappedFileTest(),
                Constants.DICTIONARY_TEST => new DictionaryTest(),
                _ => throw new NotImplementedException()
            };

            test.DoTest();
        }
    }

    public class Constants
    {
        public const string ENUMS_DESCRIPTION_TEST = "获取枚举信息测试，对比反射速度以及字典速度";

        public const string STATIC_FIELD_TEST = "静态变量测试，主要是为了测试其初始化时机，初始化的线程安全性，容量上限";

        public const string DYNAMIC_TYPE_SERIALIZE = "动态类型序列化效率测试";

        public const string TEXT_JSON_CUSTOM = "System.Text.Json自定义序列化/反序列化测试";

        public const string HTML_READER = "测试使用XmlReader对HTML进行解析及转换";

        public const string REFLECTION_CACHE = "Reflection of property with or without cache";

        public const string CHANNEL_TEST = "compare Channel with ConcurrentQueue";

        public const string NAMEDPIPE_TEST = "NamedPipe test";

        public const string MEMORYMAPPEDFILE_TEST = "MemoryMappedFile test";

        public const string DICTIONARY_TEST = "use different types key in Dictionary";
    }
}
