using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace Samples.ConsoleApp.Tests
{
    internal class DictionaryTest : ITest
    {
        Dictionary<StructKey, int> _structKeyValues = new Dictionary<StructKey, int>();
        Dictionary<ClassKey, int> _classKeyValues = new Dictionary<ClassKey, int>();
        Dictionary<CustomHashCodeClassKey, int> _customHashCodeClassKeyValues = new Dictionary<CustomHashCodeClassKey, int>();

        public void DoTest()
        {
            var keyA = new StructKey() { ProjectCode = "1", Seconds = 2 };
            var keyB = new StructKey() { ProjectCode = "1", Seconds = 2 };

            Console.WriteLine(keyA.GetHashCode());
            Console.WriteLine(keyB.GetHashCode());

            var keyC = new ClassKey() { ProjectCode = "1", Seconds = 2 };
            var keyD = new ClassKey() { ProjectCode = "1", Seconds = 2 };

            Console.WriteLine(keyC.GetHashCode());
            Console.WriteLine(keyD.GetHashCode());

            var keyE = new CustomHashCodeClassKey() { ProjectCode = "1", Seconds = 2 };
            var keyF = new CustomHashCodeClassKey() { ProjectCode = "1", Seconds = 2 };

            Console.WriteLine(keyE.GetHashCode());
            Console.WriteLine(keyF.GetHashCode());

            Console.WriteLine(keyE.Equals(keyF));
            Console.WriteLine(keyE == keyF);
            Console.WriteLine(ReferenceEquals(keyE, keyF));

            for (int i = 0; i < _targetCount; i++)
            {
                _structKeyValues[new StructKey
                {
                    Seconds = i
                }] = i;
            }

            for (int i = 0; i < _targetCount; i++)
            {
                _classKeyValues[new ClassKey
                {
                    Seconds = i
                }] = i;
            }

            for (int i = 0; i < _targetCount; i++)
            {
                _customHashCodeClassKeyValues[new CustomHashCodeClassKey
                {
                    Seconds = i
                }] = i;
            }

            _customHashCodeClassKeyValues[new CustomHashCodeClassKey { Seconds = 0 }] = 0;
            _customHashCodeClassKeyValues[new CustomHashCodeClassKey { Seconds = 1 }] = 1;
            _customHashCodeClassKeyValues[new CustomHashCodeClassKey { Seconds = 2 }] = 2;
            Console.WriteLine(_customHashCodeClassKeyValues.Count);


            var stopwatch = new Stopwatch();
            stopwatch.Restart();

            for (int i = 0; i < _checkTimes; i++)
            {
                foreach (var item in _structKeyValues)
                {
                    var value = _structKeyValues[item.Key];
                }
            }

            Console.WriteLine($"StructKey check times: {_targetCount * _checkTimes}, {stopwatch.ElapsedMilliseconds}ms");

            stopwatch.Restart();

            for (int i = 0; i < _checkTimes; i++)
            {
                foreach (var item in _classKeyValues)
                {
                    var value = _classKeyValues[item.Key];
                }
            }

            Console.WriteLine($"ClassKey check times: {_targetCount * _checkTimes}, {stopwatch.ElapsedMilliseconds}ms");

            stopwatch.Restart();

            for (int i = 0; i < _checkTimes; i++)
            {
                foreach (var item in _customHashCodeClassKeyValues)
                {
                    var value = _customHashCodeClassKeyValues[item.Key];
                }
            }

            Console.WriteLine($"CustomHashCodeClassKey check times: {_targetCount * _checkTimes}, {stopwatch.ElapsedMilliseconds}ms");
        }

        private const int _targetCount = 100_000;
        private const int _checkTimes = 1000;

        public struct StructKey
        {
            public string ProjectCode { get; set; }

            public int Seconds { get; set; }
        }

        public class ClassKey
        {
            public string ProjectCode { get; set; }

            public int Seconds { get; set; }
        }


        public class CustomHashCodeClassKey : ClassKey
        {
            public string ThreadNumber { get; set; }

            public override int GetHashCode()
            {
                return Seconds;
            }

            public override bool Equals(object obj)
            {
                if (obj is CustomHashCodeClassKey)
                {
                    var p = (CustomHashCodeClassKey)obj;
                    return Seconds == p.Seconds;
                }
                else
                {
                    return false;
                }
            }
        }
    }
}
