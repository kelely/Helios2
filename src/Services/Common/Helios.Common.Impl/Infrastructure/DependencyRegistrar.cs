using Autofac;
using Autofac.Core;
using Common.Logging;
using Helios.Caching;
using Helios.Common.Services;
using Helios.Configuration;
using Helios.Infrastructure;
using Helios.Infrastructure.DependencyManagement;

namespace Helios.Common.Infrastructure
{
    public class DependencyRegistrar : IDependencyRegistrar
    {
        private static readonly ILog Logger = LogManager.GetLogger<DependencyRegistrar>();

        public void Register(ContainerBuilder builder, ITypeFinder typeFinder, HeliosConfig config)
        {
            Logger.Debug("注册通用业务组件服务");

            builder.RegisterType<GenericAttributeService>().AsImplementedInterfaces()
                .WithParameter(ResolvedParameter.ForNamed<ICacheManager>("helios_cache_static"))
                .InstancePerLifetimeScope();

            builder.RegisterType<SettingService>().AsImplementedInterfaces()
                .WithParameter(ResolvedParameter.ForNamed<ICacheManager>("helios_cache_static"))
                .InstancePerLifetimeScope();
        }

        public int Order => 10;
    }
}
