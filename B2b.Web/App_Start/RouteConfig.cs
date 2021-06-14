using System.Web.Mvc;
using System.Web.Routing;

namespace B2b.Web.v4
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Login", action = "Index", id = UrlParameter.Optional },
               namespaces: new[] { "B2b.Web.v4.Controllers" }
            );
        }
    }
}
