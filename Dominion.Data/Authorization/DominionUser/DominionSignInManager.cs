using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using Dominion.Data.Authorization;

namespace Dominion.Data.Authorization.User.DominionUser
{
    public class DominionSignInManager : SignInManager<DominionUser, string>
    {
        public DominionSignInManager(DominionUserManager userManager, IAuthenticationManager authenticationManager)
            : base(userManager, authenticationManager)
        {
        }



        public static DominionSignInManager Create(IdentityFactoryOptions<DominionSignInManager> options, IOwinContext context)
        {
            return new DominionSignInManager(context.GetUserManager<DominionUserManager>(), context.Authentication);
        }
    }
}
