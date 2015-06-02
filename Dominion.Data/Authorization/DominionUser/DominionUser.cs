using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Microsoft.AspNet.Identity;

namespace Dominion.Data.Authorization.User.DominionUser
{
    /// <summary>
    /// Implementation of IdentityUser, used by Dominion project
    /// for all users.
    /// </summary>
   public class DominionUser : UserIdentity
    {
       public DominionUser(string email, string password)
           :base(email, password)
       {

       }

       /// <summary>
       /// User Code
       /// This Code is automatically generated upon account generation.
       /// It creates a composite key together with username.
       /// Any one username + Code is unique
       /// Lock assets to avoid multiple account registrations to occur at once for a username
       /// </summary>
       [Range(1000, 9999)]
       public virtual int Code { get; set; }

       public async Task<ClaimsIdentity> GenerateUserIdentityAsync(DominionUserManager manager)
       {
           // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
           var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
           // Add custom user claims here
           return userIdentity;
       }

    }
}
