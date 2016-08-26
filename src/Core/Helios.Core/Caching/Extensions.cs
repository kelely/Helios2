using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Helios.Caching
{
    /// <summary>
    /// 缓存管理器扩展方法
    /// </summary>
    public static class CacheExtensions
    {
        /// <summary>
        /// 从缓存中获取与指定键关联的缓存对象，如果不存在与指定键关联的缓存对象，就会使用回调方法读取数据，然后缓存起来(默认缓存10分钟)
        /// </summary>
        /// <typeparam name="T">缓存对象类型</typeparam>
        /// <param name="cacheManager">缓存控制器</param>
        /// <param name="key">缓存对象关联的键值</param>
        /// <param name="acquire">当缓存不存在时通过此匿名方法读取数据</param>
        /// <returns>缓存中的对象</returns>
        public static T Get<T>(this ICacheManager cacheManager, string key, Func<T> acquire)
        {
            return Get(cacheManager, key, 10, acquire);
        }

        /// <summary>
        /// 从缓存中获取与指定键关联的缓存对象，如果不存在与指定键关联的缓存对象，就会使用回调方法读取数据，然后缓存起来
        /// </summary>
        /// <typeparam name="T">缓存对象类型</typeparam>
        /// <param name="cacheManager">缓存控制器</param>
        /// <param name="key">缓存对象关联的键值</param>
        /// <param name="cacheTime">缓存的生命周期(单位:分钟)</param>
        /// <param name="acquire">当缓存不存在时通过此匿名方法读取数据</param>
        /// <returns>缓存中的对象</returns>
        public static T Get<T>(this ICacheManager cacheManager, string key, int cacheTime, Func<T> acquire)
        {
            if (cacheManager.IsSet(key))
            {
                return cacheManager.Get<T>(key);
            }

            var result = acquire();
            if (cacheTime > 0)
                cacheManager.Set(key, result, cacheTime);
            return result;
        }

        /// <summary>
        /// 从缓存中批量删除在给定的键值集合中与指定正则表达式匹配的的缓存对象
        /// </summary>
        /// <param name="cacheManager">缓存控制器</param>
        /// <param name="pattern">正则表达式</param>
        /// <param name="keys">键值集合</param>
        public static void RemoveByPattern(this ICacheManager cacheManager, string pattern, IEnumerable<string> keys)
        {
            var regex = new Regex(pattern, RegexOptions.Singleline | RegexOptions.Compiled | RegexOptions.IgnoreCase);
            foreach (var key in keys.Where(p => regex.IsMatch(p.ToString())).ToList())
                cacheManager.Remove(key);
        }
    }
}
