using System.Runtime.Serialization;

namespace Helios.Domain.Common
{
    /// <summary>
    /// 提供按租户隔离的配置信息实体
    /// </summary>
    [DataContract]
    public class Setting : TenantBaseEntity
    {
        /// <summary>
        /// 配置名称
        /// </summary>
        [DataMember]
        public string Name { get; set; }

        /// <summary>
        /// 配置的值
        /// </summary>
        [DataMember]
        public string Value { get; set; }

        public override string ToString()
        {
            return $"{this.TenantId}#{this.Name}:{this.Value}";
        }
    }
}
