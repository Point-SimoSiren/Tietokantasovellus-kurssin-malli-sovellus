using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace TilausDBMVC
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );

            //lisätty 25.5.2021
            routes.MapRoute(
               name: "Default2",
               url: "{culture}/{controller}/{action}/{id}",
               defaults: new
               {
                   culture = "en",
                   controller = "Tilaukset",
                   action = "TilausTiedot",
                   id = UrlParameter.Optional
               },
               constraints: new { culture = "en|fi" }
           );
        }
    }
}
