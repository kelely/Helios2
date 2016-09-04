using System.Runtime.Serialization;

namespace Helios.Domain
{
    [DataContract]
    public abstract class TenantBaseEntity : BaseEntity
    {
        /// <summary>
        /// 实体数据归属的租户Id，当租户Id为0时，代表该配置项是全体租户共用的默认值
        /// </summary>
        [DataMember]
        public int TenantId { get; set; }

        public override string ToString()
        {
            return $"{this.GetType().FullName}[#{this.Id} @{this.TenantId}]";
        }
    }
}