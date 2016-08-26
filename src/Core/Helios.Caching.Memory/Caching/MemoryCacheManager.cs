﻿using System;
using System.Linq;
using System.Runtime.Caching;

namespace Helios.Caching
{
    /// <summary>
    /// 提供运行在本地进程内的内存缓存
    /// </summary>
    public partial class MemoryCacheManager : ICacheManager
    {
        protected ObjectCache Cache => MemoryCache.Default;

        /// <summary>
        /// 从缓存中获取与指定键关联的缓存对象。
        /// </summary>
        /// <typeparam name="T">缓存对象类型</typeparam>
        /// <param name="key">缓存对象关联的键值</param>
        /// <returns>指定键关联的缓存对象</returns>
        public virtual T Get<T>(string key)
        {
            return (T)Cache[key];
        }

        /// <summary>
        /// 将指定的键和对象添加到缓存中。 
        /// </summary>
        /// <param name="key">缓存对象关联的键值</param>
        /// <param name="value">需要缓存的数据对象</param>
        /// <param name="cacheTime">缓存的生命周期(单位:分钟)</param>
        public virtual void Set(string key, object value, int cacheTime)
        {
            if (value == null)
                return;

            var policy = new CacheItemPolicy();
            policy.AbsoluteExpiration = DateTime.Now + TimeSpan.FromMinutes(cacheTime);
            Cache.Add(new CacheItem(key, value), policy);
        }

        /// <summary>
        /// 检查是否存在与指定键关联的缓存对象
        /// </summary>
        /// <param name="key">缓存对象关联的键值</param>
        public virtual bool IsSet(string key)
        {
            return (Cache.Contains(key));
        }

        /// <summary>
        /// 从缓存中删除与指定键关联的缓存对象
        /// </summary>
        /// <param name="key">缓存对象关联的键值</param>
        public virtual void Remove(string key)
        {
            Cache.Remove(key);
        }

        /// <summary>
        /// 从缓存中批量删除键值与指定正则表达式匹配的的缓存对象
        /// </summary>
        /// <param name="pattern">正则表达式</param>
        public virtual void RemoveByPattern(string pattern)
        {
            this.RemoveByPattern(pattern, Cache.Select(p => p.Key));
        }

        /// <summary>
        /// 清除所有缓存数据
        /// </summary>
        public virtual void Clear()
        {
            foreach (var item in Cache)
                Remove(item.Key);
        }

        public virtual void Dispose()
        {
        }
    }
}