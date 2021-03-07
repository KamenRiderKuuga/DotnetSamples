using System;

namespace Samples.Framework.QueueHandler
{
    public class RetryInfo
    {
        public RetryInfo(string id)
        {
            Id = id;
            RetryTime = DateTime.Now;
            HandleTimes = 0;
        }

        /// <summary>
        /// 待处理记录Id，对于每个Model来说是随机生成且不重复的
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 重试时间(处理时若已到达重试时间就开始进行处理)
        /// </summary>
        public DateTime RetryTime { get; set; }

        /// <summary>
        /// 已尝试处理的次数
        /// </summary>
        public int HandleTimes { get; set; }
    }
}
