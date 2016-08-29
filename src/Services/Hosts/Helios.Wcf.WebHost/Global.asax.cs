using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Routing;
using System.Web.Security;
using System.Web.SessionState;
using Common.Logging;
using Helios.Infrastructure;
using Helios.Web.Routes;

namespace Helios.Wcf.WebHost
{
    public class MvcApplication : System.Web.HttpApplication
    {
        private static readonly ILog Logger = LogManager.GetLogger<MvcApplication>();

        public static void RegisterRoutes(RouteCollection routes)
        {
            var routePublisher = EngineContext.Current.Resolve<IRoutePublisher>();
            routePublisher.RegisterRoutes(routes);
        }

        protected void Application_Start(object sender, EventArgs e)
        {
            Logger.Debug("");
            Logger.Debug("");
            Logger.Debug("================================================================");
            Logger.Debug("Helios 服务正在启动...");

            Stopwatch sw = new Stopwatch();
            sw.Start();

            EngineContext.Initialize(false);
            RegisterRoutes(RouteTable.Routes);

            sw.Stop();
            Logger.Debug($"服务启动成功，耗时 {sw.ElapsedMilliseconds}毫秒");
            Logger.Debug("================================================================");
        }

        protected void Application_Error(object sender, EventArgs e)
        {
            var exception = Server.GetLastError();

            if (exception == null)
                return;

            Logger.Fatal(exception.Message, exception);
        }
    }
}