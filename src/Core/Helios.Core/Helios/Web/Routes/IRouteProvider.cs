using System.Web.Routing;

namespace Helios.Web.Routes
{
    public interface IRouteProvider
    {
        void RegisterRoutes(RouteCollection routes);

        int Priority { get; }
    }
}
