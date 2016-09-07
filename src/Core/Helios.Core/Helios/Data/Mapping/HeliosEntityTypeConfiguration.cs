using System.Data.Entity.ModelConfiguration;
using Helios.Domain;

namespace Helios.Data.Mapping
{
    public abstract class HeliosEntityTypeConfiguration<T> : EntityTypeConfiguration<T> where T : BaseEntity
    {

        protected HeliosEntityTypeConfiguration()
        {
            this.HasKey(entity => entity.Id).Property(entity => entity.Id).HasColumnName("id");

            this.PostInitialize();
        }

        /// <summary>
        /// Developers can override this method in custom partial classes
        /// in order to add some custom initialization code to constructors
        /// </summary>
        protected virtual void PostInitialize()
        {
        }
    }
}