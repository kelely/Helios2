using Autofac;
using Common.Logging;
using Helios.Caching;
using Helios.Configuration;
using Helios.Infrastructure.DependencyManagement;

namespace Helios.Infrastructure.Caching
{
    public class DependencyRegistrar : IDependencyRegistrar
    {
        public virtual void Register(ContainerBuilder builder, ITypeFinder typeFinder, HeliosConfig config)
        {
            //static cache manager
            builder.RegisterType<MemoryCacheManager>()
                .As<ICacheManager>()
                .Named<ICacheManager>("helios_cache_static")
                .SingleInstance();
        }

        public int Order => -997;
    }
}