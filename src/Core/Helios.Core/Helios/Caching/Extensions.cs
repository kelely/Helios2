using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Helios.Caching
{
    /// <summary>
    /// �����������չ����
    /// </summary>
    public static class CacheExtensions
    {
        /// <summary>
        /// �ӻ����л�ȡ��ָ���������Ļ�����������������ָ���������Ļ�����󣬾ͻ�ʹ�ûص�������ȡ���ݣ�Ȼ�󻺴�����(Ĭ�ϻ���10����)
        /// </summary>
        /// <typeparam name="T">�����������</typeparam>
        /// <param name="cacheManager">���������</param>
        /// <param name="key">�����������ļ�ֵ</param>
        /// <param name="acquire">�����治����ʱͨ��������������ȡ����</param>
        /// <returns>�����еĶ���</returns>
        public static T Get<T>(this ICacheManager cacheManager, string key, Func<T> acquire)
        {
            return Get(cacheManager, key, 10, acquire);
        }

        /// <summary>
        /// �ӻ����л�ȡ��ָ���������Ļ�����������������ָ���������Ļ�����󣬾ͻ�ʹ�ûص�������ȡ���ݣ�Ȼ�󻺴�����
        /// </summary>
        /// <typeparam name="T">�����������</typeparam>
        /// <param name="cacheManager">���������</param>
        /// <param name="key">�����������ļ�ֵ</param>
        /// <param name="cacheTime">�������������(��λ:����)</param>
        /// <param name="acquire">�����治����ʱͨ��������������ȡ����</param>
        /// <returns>�����еĶ���</returns>
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
        /// �ӻ���������ɾ���ڸ����ļ�ֵ��������ָ��������ʽƥ��ĵĻ������
        /// </summary>
        /// <param name="cacheManager">���������</param>
        /// <param name="pattern">������ʽ</param>
        /// <param name="keys">��ֵ����</param>
        public static void RemoveByPattern(this ICacheManager cacheManager, string pattern, IEnumerable<string> keys)
        {
            var regex = new Regex(pattern, RegexOptions.Singleline | RegexOptions.Compiled | RegexOptions.IgnoreCase);
            foreach (var key in keys.Where(p => regex.IsMatch(p.ToString())).ToList())
                cacheManager.Remove(key);
        }
    }
}
