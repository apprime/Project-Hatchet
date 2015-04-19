using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Dominion.Web.Controllers
{
    /// <summary>
    /// Information about Minions
    /// Content Type: Static
    /// Session: Optional
    /// www.url.com/Minons
    /// </summary>
    [AllowAnonymous]
    [RoutePrefix("minions")]
    public class MinionsController : Controller
    {
        /// <summary>
        /// Base Minion page with option to search for minions
        /// Url template: www.url.com/Minions
        /// </summary>
        /// <returns>HTML View</returns>
        [Route]
        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// Information page regarding a specific minion type
        /// Url template: www.url.com/Minions/SpecificMinion
        /// </summary>
        /// <param name="name">The name of minon (String) </param>
        /// <returns>HTML View</returns>
        [Route("{name}")]
        [HttpGet]
        public ActionResult Get(string name)
        {
            return View();
        }

        //Todo: Add 404-page, query POST and query list-page
    }
}