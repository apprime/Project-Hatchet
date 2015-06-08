using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Dominion.Web.Controllers
{
    /// <summary>
    /// Handles the default/login page
    /// </summary>
    [AllowAnonymous]
    public class HomeController : Controller
    {
        [Route]
        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }
    }
}