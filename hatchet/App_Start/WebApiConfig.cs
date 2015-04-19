using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace Dominion.Web
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            //Todo(Apprime): Should API be exposed in Web project or not?
            //               If so, back-end will need to be instantiated in web proj

            // Web API configuration and services

            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );


        }
    }
}
