using System.Collections.Generic;

namespace Samples.Console.Tests
{
    public class StaticFieldTest : ITest
    {
        private static readonly List<string> _dic = InitDictionary();

        public void DoTest()
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
}
