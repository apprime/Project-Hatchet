using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using Dominion.Data.Authorization;
using Dominion.Web.Security;

namespace Dominion.Web
{
    //// Configure the application sign-in manager which is used in this application.
    //public class DominionSignInManager : SignInManager<DominionUser, string>
    //{
    //    public DominionSignInManager(DominionUserManager userManager, IAuthenticationManager authenticationManager)
    //        : base(userManager, authenticationManager)
    //    {
    //    }

    //    public override Task<ClaimsIdentity> CreateUserIdentityAsync(DominionUser user)
    //    {
    //        return user.GenerateUserIdentityAsync((DominionUserManager)UserManager);
    //    }

    //    public static DominionSignInManager Create(IdentityFactoryOptions<DominionSignInManager> options, IOwinContext context)
    //    {
    //        return new DominionSignInManager(context.GetUserManager<DominionUserManager>(), context.Authentication);
    //    }
    //}
}