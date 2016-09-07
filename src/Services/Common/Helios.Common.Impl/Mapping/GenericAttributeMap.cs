using Helios.Common.Domain;
using Helios.Data.Mapping;

namespace Helios.Common.Mapping
{
    public partial class GenericAttributeMap : HeliosEntityTypeConfiguration<GenericAttribute>
    {

        public GenericAttributeMap()
        {
            this.ToTable("generic_attributes");

            this.Property(ga => ga.EntityId).IsRequired().HasColumnName("entity_id");
            this.Property(ga => ga.EntityGroup).IsRequired().HasMaxLength(400).HasColumnName("entity_group");
            this.Property(ga => ga.Key).IsRequired().HasMaxLength(400).HasColumnName("key");
            this.Property(ga => ga.Value).IsRequired().HasColumnName("value");
        }
    }
}