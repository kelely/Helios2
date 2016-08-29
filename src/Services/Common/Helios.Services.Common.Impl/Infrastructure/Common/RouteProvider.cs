using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Activation;
using System.Text;
using System.Threading.Tasks;
using System.Web.Routing;
using Autofac.Integration.Wcf;
using Common.Logging;
using Helios.Services.Common;
using Helios.Web.Routes;

namespace Helios.Infrastructure.Common
{
    public class RouteProvider : IRouteProvider
    {
        private static readonly ILog Logger = LogManager.GetLogger<RouteProvider>();

        public void RegisterRoutes(RouteCollection routes)
        {
            Logger.Debug("注册领域对象扩展属性服务访问路由 /generic-attribute-service");
            routes.Add(new ServiceRoute("generic-attribute-service", new AutofacServiceHostFactory(), typeof(IGenericAttributeService)));
        }

        public int Priority => 10;
    }
}
