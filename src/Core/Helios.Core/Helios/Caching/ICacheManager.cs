using System;

namespace Helios.Caching
{
    /// <summary>
    /// 缓存控制器接口
    /// </summary>
    public interface ICacheManager : IDisposable
    {
        /// <summary>
        /// 从缓存中获取与指定键关联的缓存对象。
        /// </summary>
        /// <typeparam name="T">缓存对象类型</typeparam>
        /// <param name="key">缓存对象关联的键值</param>
        /// <returns>指定键关联的缓存对象</returns>
        T Get<T>(string key);

        /// <summary>
        /// 将指定的键和对象添加到缓存中。 
        /// </summary>
        /// <param name="key">缓存对象关联的键值</param>
        /// <param name="value">需要缓存的数据对象</param>
        /// <param name="cacheTime">缓存的生命周期(单位:分钟)</param>
        void Set(string key, object value, int cacheTime);

        /// <summary>
        /// 检查是否存在与指定键关联的缓存对象
        /// </summary>
        /// <param name="key">缓存对象关联的键值</param>
        bool IsSet(string key);

        /// <summary>
        /// 从缓存中删除与指定键关联的缓存对象
        /// </summary>
        /// <param name="key">缓存对象关联的键值</param>
        void Remove(string key);

        /// <summary>
        /// 从缓存中批量删除键值与指定正则表达式匹配的的缓存对象
        /// </summary>
        /// <param name="pattern">正则表达式</param>
        void RemoveByPattern(string pattern);

        /// <summary>
        /// 清除所有缓存数据
        /// </summary>
        void Clear();
    }
}
