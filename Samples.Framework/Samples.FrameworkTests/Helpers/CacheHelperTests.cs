using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Diagnostics.CodeAnalysis;

namespace Samples.Framework.Helpers.Tests
{
    [TestClass()]
    public class CacheHelperTests
    {
        /// <summary>
        /// 一个学生类，测试用
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

        [TestMethod()]
        public void TestSetAddGetCaches()
        {
            CacheHelper.Set("Number", null);
            CacheHelper.Set("Number", 2);

            Student student = new Student() { Age = 18, Name = "新学生" };
            CacheHelper.Set("Student", student);

            string tempValue = "NewValue";
            CacheHelper.Set("StringValue", tempValue);

            Assert.AreEqual(3, CacheHelper.Instance.GetCount());
            Assert.AreEqual(2, CacheHelper.Get<int>("Number"));
            Assert.IsTrue(student.Equals(CacheHelper.Get<Student>("Student")));
            Assert.AreEqual("NewValue", CacheHelper.Get<string>("StringValue"));

            CacheHelper.Set("Student", student, 1);
            // Sleep一分钟，然后查看结果
            System.Threading.Thread.Sleep(60000);

        }
    }
}