using System;
using System.Web.Mvc;
using System.Web;
using Dominion.Web.Security;

namespace Dominion.Web.Controllers
{
    public class MainController : Controller
    {
        //This shouldn't exist. Instead we need to redirect to a websockets connection
        [AuthorizeCustom]
        public ActionResult Index()
        {
            return View("main");
        }
    }
}