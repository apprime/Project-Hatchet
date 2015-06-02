using Dominion.Data.Validation.Regex;
using Microsoft.AspNet.Identity;
using System;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Dominion.Data.Authorization.User
{
    //Todo(apprime): Ensure all values in user are populated or default in constructor. Avoid nulls in and out of db!
    /// <summary>
    /// Handles the information about a user that is needed for authorization
    /// Inherits from UserHandle that is needed to uniquely identify a user
    ///  TODO: Move this and all baseline Identity to some sort of library or secluded part of project
    /// </summary>
    public class UserIdentity : IUser<string>
    {
        /// <summary>
        /// Default constructor 
        /// </summary>
        public UserIdentity()
        {
        }

        /// <summary>
        /// Constructor for new Users
        /// </summary>
        /// <param name="userName">(String)</param>
        /// <param name="email">(String)</param>
        /// <param name="password">(String)</param>
        public UserIdentity(string email, string password) //Todo(apprime): securestring password?
        {
            LockoutEnabled = true; //TODO: Magic value, set as default constant in webconfig
            PhoneNumberConfirmed = false;
        }

        public virtual async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<UserIdentity, string> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }

        /// <summary>
        /// UserName as a string
        /// </summary>
        [RegularExpression(Common.Alphanumeric)]
        public virtual string UserName { get; set; }
        
        /// <summary>
        /// Unique identifier for a user
        /// </summary>
        public virtual string Id { get; set; }

        /// <summary>
        /// Email is the unique Id for a user. 
        /// It is simply a synonym
        /// </summary>
        [EmailAddress]
        [Required]
        public virtual string Email 
        { 
            get { return Id; }
            set { this.Id = value; } 
        }

        /// <summary>
        ///     True if the email is confirmed, default is false
        /// </summary>
        [Required]
        public virtual bool EmailConfirmed { get; set; }

        /// <summary>
        ///     The salted/hashed form of the user password
        /// </summary>
        [Required]
        public virtual string PasswordHash { get; set; }

        /// <summary>
        ///     A random value that should change whenever a users credentials have changed (password changed, login removed)
        /// </summary>
        public virtual string SecurityStamp { get; set; }

        #region Two-Step Verification
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
        #endregion

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
        ///     How many attempts are allowed until user
        /// </summary>
        public virtual bool LockoutLimit { get; set; }
      
        /// <summary>
        /// Used to record failures for the purposes of lockout
        /// </summary>
        public virtual int AccessFailedCount { get; set; }
        #endregion
    }
}
