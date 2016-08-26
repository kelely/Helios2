using System;

namespace Helios.Caching
{
    /// <summary>
    /// ����������ӿ�
    /// </summary>
    public interface ICacheManager : IDisposable
    {
        /// <summary>
        /// �ӻ����л�ȡ��ָ���������Ļ������
        /// </summary>
        /// <typeparam name="T">�����������</typeparam>
        /// <param name="key">�����������ļ�ֵ</param>
        /// <returns>ָ���������Ļ������</returns>
        T Get<T>(string key);

        /// <summary>
        /// ��ָ���ļ��Ͷ�����ӵ������С� 
        /// </summary>
        /// <param name="key">�����������ļ�ֵ</param>
        /// <param name="value">��Ҫ��������ݶ���</param>
        /// <param name="cacheTime">�������������(��λ:����)</param>
        void Set(string key, object value, int cacheTime);

        /// <summary>
        /// ����Ƿ������ָ���������Ļ������
        /// </summary>
        /// <param name="key">�����������ļ�ֵ</param>
        bool IsSet(string key);

        /// <summary>
        /// �ӻ�����ɾ����ָ���������Ļ������
        /// </summary>
        /// <param name="key">�����������ļ�ֵ</param>
        void Remove(string key);

        /// <summary>
        /// �ӻ���������ɾ����ֵ��ָ��������ʽƥ��ĵĻ������
        /// </summary>
        /// <param name="pattern">������ʽ</param>
        void RemoveByPattern(string pattern);

        /// <summary>
        /// ������л�������
        /// </summary>
        void Clear();
    }
}
