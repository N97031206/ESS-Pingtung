using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Web
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Account", action = "Login", id = UrlParameter.Optional },
                //defaults: new { controller = "Home", action = "Default", id = UrlParameter.Optional },

                //避免錯誤發生：Ambiguous controller
                //因為前後台有名稱相同的Controller
                namespaces: new string[] { "Web.Controllers" }
            );
        }
    }
}
