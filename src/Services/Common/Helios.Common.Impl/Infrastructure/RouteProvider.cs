using System.ServiceModel.Activation;
using System.Web.Routing;
using Autofac.Integration.Wcf;
using Common.Logging;
using Helios.Common.Services;
using Helios.Web.Routes;

namespace Helios.Common.Infrastructure
{
    public class RouteProvider : IRouteProvider
    {
        private static readonly ILog Logger = LogManager.GetLogger<RouteProvider>();

        public void RegisterRoutes(RouteCollection routes)
        {
            Logger.Debug("注册领域对象扩展属性服务访问路由 /generic-attribute-service");
            routes.Add(new ServiceRoute("generic-attribute-service", new AutofacServiceHostFactory(), typeof(IGenericAttributeService)));

            Logger.Debug("注册租户配置信息管理服务访问路由 /setting-service");
            routes.Add(new ServiceRoute("setting-service", new AutofacServiceHostFactory(), typeof(ISettingService)));
        }

        public int Priority => 10;
    }
}
