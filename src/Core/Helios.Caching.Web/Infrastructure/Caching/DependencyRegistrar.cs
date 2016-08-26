using Autofac;
using Helios.Caching;
using Helios.Configuration;
using Helios.Infrastructure.DependencyManagement;

namespace Helios.Infrastructure.Caching
{
    public class DependencyRegistrar : IDependencyRegistrar
    {
        public virtual void Register(ContainerBuilder builder, ITypeFinder typeFinder, HeliosConfig config)
        {
            builder.RegisterType<PerRequestCacheManager>()
                .As<ICacheManager>()
                .Named<ICacheManager>("helios_cache_per_request")
                .InstancePerLifetimeScope();
        }

        public int Order => -997;
    }
}