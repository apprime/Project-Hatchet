using System;
using System.Web.Mvc;
using System.Web.Routing;

namespace Dominion.Web.Security
{
    /// <summary>
    /// Overrides the Authorize attribute with custom implementations
    /// </summary>
    public class AuthorizeCustomAttribute : AuthorizeAttribute
    {
        /// <summary>
        /// Redirect to index without appending returnUrl to query,
        /// </summary>
        /// <param name="filterContext">AuthorizationContext</param>
        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            filterContext.Result = new RedirectToRouteResult
                                    (
                                        new RouteValueDictionary 
                                            {
                                                { "action", "Index" },
                                                { "controller", "Home" }
                                            }
                                    );
        }
    }
}