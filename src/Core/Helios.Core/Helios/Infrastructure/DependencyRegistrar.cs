using Autofac;
using Common.Logging;
using Helios.Configuration;
using Helios.Infrastructure.DependencyManagement;
using Helios.Web;
using Helios.Web.Routes;

namespace Helios.Infrastructure
{
    public class DependencyRegistrar :IDependencyRegistrar
    {
        private static readonly ILog Logger = LogManager.GetLogger<DependencyRegistrar>();

        public void Register(ContainerBuilder builder, ITypeFinder typeFinder, HeliosConfig config)
        {
            Logger.Debug("注册 Helios 核心接口");
            builder.RegisterType<WebHelper>().As<IWebHelper>().SingleInstance();
            builder.RegisterType<RoutePublisher>().As<IRoutePublisher>().SingleInstance();
        }

        public int Order => -999;
    }
}
