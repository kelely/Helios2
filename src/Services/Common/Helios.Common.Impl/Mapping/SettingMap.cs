using Helios.Common.Domain;
using Helios.Data.Mapping;

namespace Helios.Common.Mapping
{
    public partial class SettingMap : HeliosEntityTypeConfiguration<Setting>
    {
        public SettingMap()
        {
            this.ToTable("settings");

            this.Property(s => s.Name).IsRequired().HasMaxLength(200).HasColumnName("name");
            this.Property(s => s.Value).IsRequired().HasMaxLength(2000).HasColumnName("value");
            this.Property(s => s.TenantId).IsRequired().HasColumnName("tenant_id");
        }
    }
}