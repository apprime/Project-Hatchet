using Microsoft.AspNet.Identity;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Security.Claims;
using System.Security;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;  
using Dominion.Data.Authorization;

namespace Dominion.Data.Authorization.User
{
    //Todo(apprime): Ensure all values in user are populated or default in constructor. Avoid nulls in and out of db!
    /// <summary>
    /// Class that implements the ASP.NET Identity
    /// IUser interface 
    /// </summary>
    public class DominionUser : IUser<int>
    {
        /// <summary>
        /// Default constructor 
        /// </summary>
        public DominionUser()
        {
        }

        /// <summary>
        /// Constructor for new Users
        /// </summary>
        /// <param name="userName">(String)</param>
        /// <param name="email">(String)</param>
        /// <param name="password">(String)</param>
        public DominionUser(string userName, int userId, string email, string password) //Todo(apprime): securestring password?
            : this()
        {
            UserName = userName;
            LockoutEnabled = true; //TODO: Magic number, set as default constant in webconfig
            Id = userId;
        }

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(DominionUserManager manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }

        /// <summary>
        /// User ID
        /// This Id is automatically generated upon account generation.
        /// It creates a composite key together with username.
        /// Any one username + id is unique
        /// Lock assets to avoid multiple account registrations to occur at once for a username
        /// </summary>
        [Range(1000, 9999)]
        public int Id { get; set; }

        /// <summary>
        /// User's name
        /// </summary>
        //Todo(apprime): Alphanumeric only!
        public string UserName { get; set; }

        /// <summary>
        ///     Email
        /// </summary>
        [EmailAddress]
        [Required]
        public virtual string Email { get; set; }

        /// <summary>
        ///     True if the email is confirmed, default is false
        /// </summary>
        [Required]
        public virtual bool EmailConfirmed { get; set; }

        /// <summary>
        ///     The salted/hashed form of the user password
        /// </summary>
        public virtual string PasswordHash { get; set; }

        /// <summary>
        ///     A random value that should change whenever a users credentials have changed (password changed, login removed)
        /// </summary>
        public virtual string SecurityStamp { get; set; }

        /// <summary>
        ///     PhoneNumber for the user
        /// </summary>
        //Todo(apprime): Get custom validator for phone-numbers AND add default phonenr to avoid dbnulls
        public virtual string PhoneNumber { get; set; }

        /// <summary>
        ///     True if the phone number is confirmed, default is false
        /// </summary>
        public virtual bool PhoneNumberConfirmed { get; set; }

        /// <summary>
        ///     Is two factor enabled for the user
        /// </summary>
        public virtual bool TwoFactorEnabled { get; set; }

        #region Lockout Variables
        /// <summary>
        ///     DateTime in UTC when lockout ends, any time in the past is considered not locked out.
        /// </summary>
        public virtual DateTime? LockoutEndDateUtc { get; set; }

        /// <summary>
        ///     Has user enabled account lockout?
        /// </summary>
        public virtual bool LockoutEnabled { get; set; }
      
        /// <summary>
        /// Used to record failures for the purposes of lockout
        /// </summary>
        public int AccessFailedCount { get; set; }
        #endregion
    }
}
