using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Samples.Framework.Helpers.Tests
{
    [TestClass()]
    public class CacheHelperTests
    {
        /// <summary>
        /// 一个学生类，实现了IEquatable接口，测试用
        /// </summary>
        public class Student : IEquatable<Student>
        {
            public bool Equals(Student other)
            {
                return other?.Age == Age && other?.Name == Name;
            }

            /// <summary>
            /// 年龄
            /// </summary>
            public int Age { get; set; }

            /// <summary>
            /// 姓名
            /// </summary>
            public string Name { get; set; }
        }

        /// <summary>
        /// 一个教师类，测试用
        /// </summary>
        public class Teacher
        {
            /// <summary>
            /// 年龄
            /// </summary>
            public int Age { get; set; }

            /// <summary>
            /// 姓名
            /// </summary>
            public string Name { get; set; }
        }

        private readonly string _numberField = "Number";
        private readonly string _studentField = "Student";
        private readonly string _stringField = "StringValue";
        private readonly string _teacherField = "Teacher";
        private readonly string _multiThreadField = "multiThread";

        [TestMethod("设置和获取值的测试")]
        public void TestSetAddGetCaches()
        {
            // 添加null值（MemoryCache默认不接收null值作为value）
            CacheHelper.Set(_numberField, null);

            // 使用同样的key添加两次数字
            CacheHelper.Set(_numberField, 1);
            CacheHelper.Set(_numberField, 2);

            // 添加字符串
            string tempString = "NewValue";
            CacheHelper.Set(_stringField, tempString);

            // 添加自定义对象
            Student student = new Student() { Age = 18, Name = "新学生" };
            CacheHelper.Set(_studentField, student);

            // 检查缓存内容的数量
            Assert.AreEqual(3, CacheHelper.Instance.GetCount());

            // 检查数字内容
            Assert.AreEqual(2, CacheHelper.Get<int>(_numberField));

            // 检查字符串内容
            Assert.AreEqual(tempString, CacheHelper.Get<string>(_stringField));

            // 检查自定义对象各属性是否相等
            Assert.IsTrue(student.Equals(CacheHelper.Get<Student>(_studentField)));

            // 当使用了自定义的函数进行存取，址会发生改变
            Assert.AreNotEqual(student, CacheHelper.Get<Student>(_studentField));

            // 当直接使用ObjectCache实例进行存取，址不会发生改变
            Teacher teacher = new Teacher() { Age = 1, Name = "老师A" };
            CacheHelper.Instance.Set(_teacherField, teacher, DateTimeOffset.Now.AddMinutes(1));
            teacher.Name = "老师B";
            Assert.AreEqual(teacher, CacheHelper.Instance.Get(_teacherField));
        }

        [TestMethod("过期时间的测试")]
        public void TestExpireTime()
        {
            Action loopUntilExpire = () =>
            {
                int loopTimes = 0;

                while (true)
                {
                    loopTimes++;

                    // 等待10秒
                    Thread.Sleep(new TimeSpan(0, 0, 10));

                    Console.WriteLine($"等待了{loopTimes * 10}s");

                    if (CacheHelper.Get<string>(_stringField) == null)
                    {
                        Console.WriteLine($"等待{loopTimes * 10}s之后过期");
                        break;
                    }

                    if (loopTimes >= 10)
                    {
                        Console.WriteLine($"等待{loopTimes * 10}s之后仍未过期，退出循环");
                        break;
                    }
                }
            };

            // 测试绝对过期时间，一分钟之后过期
            CacheHelper.Set(_stringField, "一分钟后过期", 60, false);
            loopUntilExpire();

            // 测试相对过期时间，10秒之后过期
            CacheHelper.Set(_stringField, "十秒后过期", 10);
            loopUntilExpire();

            // 测试相对过期时间，20秒之后过期
            CacheHelper.Set(_stringField, "二十秒后过期", 20);
            loopUntilExpire();

            Assert.IsTrue(true);
        }

        [TestMethod("测试多线程访问")]
        public void TestMultiThreadAccess()
        {
            int tryTimes = 10;

            Func<string, string, Task> newTask = (string taskName, string value) => Task.Run(() =>
            {
                CacheHelper.Set(_multiThreadField, value);
                Console.WriteLine($"线程名：{taskName}，设置的值：{value}");
                Console.WriteLine($"线程名：{taskName}，获取到的值：{CacheHelper.Get<int>(_multiThreadField)}");
            });

            while (tryTimes > 0)
            {
                tryTimes--;
                Task task1 = newTask("线程1", "1");
                Task task2 = newTask("线程2", "2");
                Task task3 = newTask("线程3", "3");
                Task.WaitAll(task1, task2, task3);
            }
        }
    }
}