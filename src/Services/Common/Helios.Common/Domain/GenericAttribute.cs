using System.Runtime.Serialization;
using Helios.Domain;

namespace Helios.Common.Domain
{
    /// <summary>
    /// Represents a generic attribute
    /// </summary>
    [DataContract]
    public class GenericAttribute : BaseEntity
    {
        /// <summary>
        /// Gets or sets the entity identifier
        /// </summary>
        [DataMember]
        public int EntityId { get; set; }

        /// <summary>
        /// Gets or sets the key group
        /// </summary>
        [DataMember]
        public string EntityGroup { get; set; }

        /// <summary>
        /// Gets or sets the key
        /// </summary>
        [DataMember]
        public string Key { get; set; }

        /// <summary>
        /// Gets or sets the value
        /// </summary>
        [DataMember]
        public string Value { get; set; }
    }
}
