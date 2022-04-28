using Samples.Console.Utils;
using System;
using System.Collections.Concurrent;
using System.ComponentModel;
using System.Reflection;

namespace Samples.Console.Tests
{
    public class ReflectionCacheTest : ITest
    {
        readonly ConcurrentDictionary<(Type, string), PropertyInfo> _propertyInfo = new ConcurrentDictionary<(Type, string), PropertyInfo>();
        readonly ConcurrentDictionary<Type, PropertyInfo> _propertyInfoWithType = new ConcurrentDictionary<Type, PropertyInfo>();
        readonly ConcurrentDictionary<string, PropertyInfo> _propertyInfoWithFullName = new ConcurrentDictionary<string, PropertyInfo>();

        private TProperty GetValueOfPropertyNoCache<TProperty>(object entity, string propertyName)
        {
            var type = entity.GetType();

            return (TProperty)type.GetProperty(propertyName).GetValue(entity);
        }

        private TProperty GetValueOfPropertyCache<TProperty, TEntity>(TEntity entity, string propertyName)
        {
            var type = entity.GetType();

            if (!_propertyInfo.TryGetValue((type, propertyName), out var propertyInfo))
            {
                propertyInfo = type.GetProperty(propertyName);
                _propertyInfo.TryAdd((type, propertyName), propertyInfo);
            }

            return (TProperty)propertyInfo.GetValue(entity);
        }

        private TProperty GetValueOfPropertyCacheType<TProperty, TEntity>(TEntity entity, string propertyName)
        {
            var type = entity.GetType();

            if (!_propertyInfoWithType.TryGetValue(type, out var propertyInfo))
            {
                propertyInfo = type.GetProperty(propertyName);
                _propertyInfoWithType.TryAdd(type, propertyInfo);
            }

            return (TProperty)propertyInfo.GetValue(entity);
        }

        private TProperty GetValueOfPropertyCacheFullName<TProperty, TEntity>(TEntity entity, string propertyName)
        {
            var type = entity.GetType();
            var key = type.FullName + "," + propertyName;

            if (!_propertyInfoWithFullName.TryGetValue(key, out var propertyInfo))
            {
                propertyInfo = type.GetProperty(propertyName);
                _propertyInfoWithFullName.TryAdd(key, propertyInfo);
            }

            return (TProperty)propertyInfo.GetValue(entity);
        }

        class SampleModel
        {
            public string Name { get; set; }

            public int Age { get; set; }

            public int Sex { get; set; }
        }

        enum SampleEnums
        {
            [Description("第一个枚举")]
            First = 1,

            [Description("第二个枚举")]
            Second = 2,

            [Description("第三个枚举")]
            Third = 3
        }

        public void DoTest()
        {
            var model = new SampleModel() { Name = "名称" };

            int tryTimes = 100000000;

            // 由于预编译的原因，这里测试两次
            for (int i = 1; i < 3; i++)
            {
                System.Console.WriteLine("第{0}轮测试，直接反射花费的时间({1}次)，{2}ms", i, tryTimes, TestUtils.TimeMethod(() =>
                {
                    for (int _ = 0; _ < tryTimes; _++)
                    {
                        GetValueOfPropertyNoCache<string>(model, "Name");
                    }
                }));

                System.Console.WriteLine("第{0}轮测试，使用字典做缓存（Type + FieldName作为键）花费的时间({1}次)，{2}ms", i, tryTimes, TestUtils.TimeMethod(() =>
                {
                    for (int _ = 0; _ < tryTimes; _++)
                    {
                        GetValueOfPropertyCache<string, SampleModel>(model, "Name");
                    }
                }));

                System.Console.WriteLine("第{0}轮测试，使用字典做缓存（Type作为键）（不足以完全界定字段）花费的时间({1}次)，{2}ms", i, tryTimes, TestUtils.TimeMethod(() =>
                {
                    for (int _ = 0; _ < tryTimes; _++)
                    {
                        GetValueOfPropertyCacheType<string, SampleModel>(model, "Name");
                    }
                }));

                System.Console.WriteLine("第{0}轮测试，使用字典做缓存（FullName + PropertyName作为键）花费的时间({1}次)，{2}ms", i, tryTimes, TestUtils.TimeMethod(() =>
                {
                    for (int _ = 0; _ < tryTimes; _++)
                    {
                        GetValueOfPropertyCacheFullName<string, SampleModel>(model, "Name");
                    }
                }));
            }
        }
    }
}
