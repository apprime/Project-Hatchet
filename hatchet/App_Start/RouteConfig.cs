using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Dominion.Web
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.IgnoreRoute("Home");
            routes.MapMvcAttributeRoutes();

            //Minions, Items, Abilities, Research, Spells, Buildings, Terrain(?), Maps
            //Index = Start page for this particular kind of data, with search/list of data items
            //Get = Gets a specific data item by name

            routes.MapRoute(
                name: "Default",
                url: "*",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
