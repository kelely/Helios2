using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Routing;
using Helios.Infrastructure;

namespace Helios.Web.Routes
{
    /// <summary>
    /// Route publisher
    /// </summary>
    public class RoutePublisher : IRoutePublisher
    {
        private readonly ITypeFinder _typeFinder;

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="typeFinder"></param>
        public RoutePublisher(ITypeFinder typeFinder)
        {
            this._typeFinder = typeFinder;
        }

        /// <summary>
        /// Register routes
        /// </summary>
        /// <param name="routes">Routes</param>
        public virtual void RegisterRoutes(RouteCollection routes)
        {
            var routeProviderTypes = _typeFinder.FindClassesOfType<IRouteProvider>();
            var routeProviders = new List<IRouteProvider>();
            foreach (var providerType in routeProviderTypes)
            {
                var provider = Activator.CreateInstance(providerType) as IRouteProvider;
                routeProviders.Add(provider);
            }
            routeProviders = routeProviders.OrderByDescending(rp => rp.Priority).ToList();
            routeProviders.ForEach(rp => rp.RegisterRoutes(routes));
        }
    }
}
