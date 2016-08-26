namespace Helios.Caching
{
    /// <summary>
    /// �ṩһ���յĻ��������(��ʹ���κλ���)
    /// </summary>
    public partial class HeliosNullCache : ICacheManager
    {
        /// <summary>
        /// �ӻ����л�ȡ��ָ���������Ļ������
        /// </summary>
        /// <typeparam name="T">�����������</typeparam>
        /// <param name="key">�����������ļ�ֵ</param>
        /// <returns>ָ���������Ļ������</returns>
        public virtual T Get<T>(string key)
        {
            return default(T);
        }

        /// <summary>
        /// ��ָ���ļ��Ͷ�����ӵ������С� 
        /// </summary>
        /// <param name="key">�����������ļ�ֵ</param>
        /// <param name="value">��Ҫ��������ݶ���</param>
        /// <param name="cacheTime">�������������(��λ:����)</param>
        public virtual void Set(string key, object data, int cacheTime)
        {
        }

        /// <summary>
        /// ����Ƿ������ָ���������Ļ������
        /// </summary>
        /// <param name="key">�����������ļ�ֵ</param>
        public bool IsSet(string key)
        {
            return false;
        }

        /// <summary>
        /// �ӻ�����ɾ����ָ���������Ļ������
        /// </summary>
        /// <param name="key">�����������ļ�ֵ</param>
        public virtual void Remove(string key)
        {
        }

        /// <summary>
        /// �ӻ���������ɾ����ֵ��ָ��������ʽƥ��ĵĻ������
        /// </summary>
        /// <param name="pattern">������ʽ</param>
        public virtual void RemoveByPattern(string pattern)
        {
        }

        /// <summary>
        /// ������л�������
        /// </summary>
        public virtual void Clear()
        {
        }

        public virtual void Dispose()
        {
        }
    }
}