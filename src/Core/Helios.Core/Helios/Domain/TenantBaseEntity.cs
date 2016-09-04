using System.Runtime.Serialization;

namespace Helios.Domain
{
    [DataContract]
    public abstract class TenantBaseEntity : BaseEntity
    {
        /// <summary>
        /// ʵ�����ݹ������⻧Id�����⻧IdΪ0ʱ���������������ȫ���⻧���õ�Ĭ��ֵ
        /// </summary>
        [DataMember]
        public int TenantId { get; set; }

        public override string ToString()
        {
            return $"{this.GetType().FullName}[#{this.Id} @{this.TenantId}]";
        }
    }
}