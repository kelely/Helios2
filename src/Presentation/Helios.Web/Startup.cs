using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(Helios.Web.Startup))]

namespace Helios.Web
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAutofac(app);
            ConfigureAuth(app);
        }
    }
}
