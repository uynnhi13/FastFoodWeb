using System.Web.Mvc;
using System.Web.Routing;

namespace TMDT
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "KhachHang",
                url: "{area}/{controller}/{action=Index}/{id}",
                defaults: new { Areas = "KhachHang", controller = "Home", action = "Index", id = UrlParameter.Optional },
                namespaces: new[] { "TMDT.Areas.KhachHang.Controllers" } // Namespace của vùng (area) KhachHang
            );
    routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional },
                namespaces: new[] { "TMDT.Controllers" } // Namespace của controller chính
            );

            routes.MapRoute(
                name: "Admin",
                url: "{area}/{controller}/{action=Index}/{id}",
                defaults: new { Areas = "Admin", controller = "Admin", action = "Login", id = UrlParameter.Optional },
                namespaces: new[] { "TMDT.Areas.Admin.Controllers" } // Namespace của vùng (area) KhachHang
            );
        }
    }
}
