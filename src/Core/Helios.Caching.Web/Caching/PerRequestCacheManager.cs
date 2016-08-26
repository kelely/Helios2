using System.Collections;
using System.Linq;
using System.Web;

namespace Helios.Caching
{
    /// <summary>
    /// 提供一个将数据缓存在HTTP请求内的缓存控制器(短期缓存,每个HTTP请求内有效)
    /// </summary>
    public class PerRequestCacheManager : ICacheManager
    {
        private readonly HttpContextBase _context;

        public PerRequestCacheManager(HttpContextBase context)
        {
            this._context = context;
        }

        protected virtual IDictionary GetItems()
        {
            if (_context == null)
                return null;

            return _context.Items;
        }

        /// <summary>
        /// 从缓存中获取与指定键关联的缓存对象。
        /// </summary>
        /// <typeparam name="T">缓存对象类型</typeparam>
        /// <param name="key">缓存对象关联的键值</param>
        /// <returns>指定键关联的缓存对象</returns>
        public virtual T Get<T>(string key)
        {
            var items = GetItems();
            if (items == null)
                return default(T);

            return (T)items[key];
        }

        /// <summary>
        /// 将指定的键和对象添加到缓存中。 
        /// </summary>
        /// <param name="key">缓存对象关联的键值</param>
        /// <param name="value">需要缓存的数据对象</param>
        /// <param name="cacheTime">缓存的生命周期(单位:分钟)</param>
        public virtual void Set(string key, object value, int cacheTime)
        {
            var items = GetItems();
            if (items == null)
                return;

            if (value == null)
                return;

            if (items.Contains(key))
                items[key] = value;
            else
                items.Add(key, value);
        }

        /// <summary>
        /// 检查是否存在与指定键关联的缓存对象
        /// </summary>
        /// <param name="key">缓存对象关联的键值</param>
        public virtual bool IsSet(string key)
        {
            var items = GetItems();

            if (items == null)
                return false;
            
            return items[key] != null;
        }

        /// <summary>
        /// 从缓存中删除与指定键关联的缓存对象
        /// </summary>
        /// <param name="key">缓存对象关联的键值</param>
        public virtual void Remove(string key)
        {
            var items = GetItems();
            if (items == null)
                return;

            items.Remove(key);
        }

        /// <summary>
        /// 从缓存中批量删除键值与指定正则表达式匹配的的缓存对象
        /// </summary>
        /// <param name="pattern">正则表达式</param>
        public virtual void RemoveByPattern(string pattern)
        {
            var items = GetItems();
            if (items == null)
                return;

            this.RemoveByPattern(pattern, items.Keys.Cast<object>().Select(p => p.ToString()));
        }

        /// <summary>
        /// 清除所有缓存数据
        /// </summary>
        public virtual void Clear()
        {
            var items = GetItems();
            if (items == null)
                return;

            items.Clear();
        }

        public virtual void Dispose()
        {
        }
    }
}
