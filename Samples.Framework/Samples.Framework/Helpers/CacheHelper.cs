using System;
using System.Runtime.Caching;

namespace Samples.Framework.Helpers
{
    public class CacheHelper
    {
        private static ObjectCache _cache = MemoryCache.Default;

        /// <summary>
        /// 返回缓存管理器的实例
        /// </summary>
        public static ObjectCache Instance => _cache;

        /// <summary>
        /// 新增(若无)/替换(若已存在)一条现有的缓存记录
        /// </summary>
        /// <param name="key">缓存的键</param>
        /// <param name="value">缓存的内容</param>
        /// <param name="expireSeconds">缓存的过期时间(单位：秒)</param>
        /// <param name="isSlidingExpire">是否滑动过期，默认为true，滑动过期即超过某个时间未访问一个键，则该键到期</param>
        public static void Set(string key, object value, int expireSeconds = 600, bool isSlidingExpire = true)
        {
            Set(key, value, new TimeSpan(0, 0, expireSeconds), isSlidingExpire);
        }

        /// <summary>
        /// 新增(若无)/替换(若已存在)一条现有的缓存记录
        /// </summary>
        /// <param name="key">缓存的键</param>
        /// <param name="value">缓存的内容</param>
        /// <param name="cacheTime">缓存的过期时间，时间间隔</param>
        /// <param name="isSlidingExpire">是否滑动过期，默认为true，滑动过期即超过某个时间未访问一个键，则该键到期</param>
        public static void Set(string key, object value, TimeSpan cacheTime, bool isSlidingExpire = true)
        {
            if (value is null)
            {
                return;
            }

            string jsonValue = Newtonsoft.Json.JsonConvert.SerializeObject(value);

            CacheItemPolicy policy = new CacheItemPolicy();

            if (isSlidingExpire)
            {
                policy.SlidingExpiration = cacheTime;
            }
            else
            {
                policy.AbsoluteExpiration = DateTimeOffset.Now.AddSeconds(cacheTime.TotalSeconds);
            }

            _cache.Set(key, jsonValue, policy);
        }

        /// <summary>
        /// 获取一条存在的缓存记录，若无，则返回指定类型的默认值
        /// </summary>
        /// <typeparam name="T">缓存内容的类型</typeparam>
        /// <param name="key">缓存的键</param>
        /// <returns></returns>
        public static T Get<T>(string key)
        {
            var cacheItem = _cache.Get(key);

            return cacheItem is null ? default(T) : Newtonsoft.Json.JsonConvert.DeserializeObject<T>(cacheItem.ToString());
        }
    }
}
