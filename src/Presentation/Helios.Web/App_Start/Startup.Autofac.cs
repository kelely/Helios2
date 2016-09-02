using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Helios.Infrastructure;
using Owin;

namespace Helios.Web
{
    public partial class Startup
    {
        public void ConfigureAutofac(IAppBuilder app)
        {
            app.UseAutofacMiddleware(EngineContext.Current.ContainerManager.Container);
        }
    }
}