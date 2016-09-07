using Autofac;
using Helios.Authentication.Services;
using Helios.Configuration;
using Helios.Infrastructure;
using Helios.Infrastructure.DependencyManagement;

namespace Helios.Web.Infrastructure
{
    public class DependencyRegistrar : IDependencyRegistrar
    {
//        private static readonly ILog Logger = LogManager.GetLogger<DependencyRegistrar>();

        public void Register(ContainerBuilder builder, ITypeFinder typeFinder, HeliosConfig config)
        {
//            Logger.Debug("注册领域对象扩展属性服务");

            builder.RegisterType<AuthenticationService>().As<IAuthenticationService>()
                .InstancePerLifetimeScope();
        }

        public int Order => 10;
    }
}
