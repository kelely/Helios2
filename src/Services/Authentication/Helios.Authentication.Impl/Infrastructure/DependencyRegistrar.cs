using Autofac;
using Autofac.Core;
using Common.Logging;
using Helios.Authentication.Services;
using Helios.Caching;
using Helios.Configuration;
using Helios.Infrastructure;
using Helios.Infrastructure.DependencyManagement;

namespace Helios.Authentication.Infrastructure
{
    public class DependencyRegistrar : IDependencyRegistrar
    {
        private static readonly ILog Logger = LogManager.GetLogger<DependencyRegistrar>();

        public void Register(ContainerBuilder builder, ITypeFinder typeFinder, HeliosConfig config)
        {
            Logger.Debug("注册用户认证业务组件服务");

            builder.RegisterType<AuthenticationService>().AsImplementedInterfaces()
//                .WithParameter(ResolvedParameter.ForNamed<ICacheManager>("helios_cache_static"))
                .InstancePerLifetimeScope();
        }

        public int Order => 10;
    }
}
