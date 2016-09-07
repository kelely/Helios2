using System.ServiceModel.Activation;
using System.Web.Routing;
using Autofac.Integration.Wcf;
using Common.Logging;
using Helios.Authentication.Services;
using Helios.Web.Routes;

namespace Helios.Authentication.Infrastructure
{
    public class RouteProvider : IRouteProvider
    {
        private static readonly ILog Logger = LogManager.GetLogger<RouteProvider>();

        public void RegisterRoutes(RouteCollection routes)
        {
            Logger.Debug("注册用户认证业务组件服务访问路由 /generic-attribute-service");
            routes.Add(new ServiceRoute("auth-service", new AutofacServiceHostFactory(), typeof(IAuthenticationService)));
        }

        public int Priority => 10;
    }
}
