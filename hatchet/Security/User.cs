﻿//using Microsoft.AspNet.Identity;
//using System;
//using System.Collections.Generic;
//using System.Security.Claims;
//using System.Threading.Tasks;
//using Dominion.Data.Authorization.User.DominionUser;

//namespace Dominion.Web.Security
//{
//    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
//    public class User : IUser
//    {
//        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(DominionUserManager manager)
//        {
//            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
//            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
//            // Add custom user claims here
//            return userIdentity;
//        }

//        /// <summary>
//        /// Default constructor 
//        /// </summary>
//        public User()
//        {
//            Id = Guid.NewGuid().ToString();
//        }

//        /// <summary>
//        /// Constructor that takes user name as argument
//        /// </summary>
//        /// <param name="userName"></param>
//        public User(string userName)
//            : this()
//        {
//            UserName = userName;
//        }

//        /// <summary>
//        /// User ID
//        /// </summary>
//        public string Id { get; set; }

//        /// <summary>
//        /// User's name
//        /// </summary>
//        public string UserName { get; set; }

//        /// <summary>
//        ///     Email
//        /// </summary>
//        public virtual string Email { get; set; }

//        /// <summary>
//        ///     True if the email is confirmed, default is false
//        /// </summary>
//        public virtual bool EmailConfirmed { get; set; }

//        /// <summary>
//        ///     The salted/hashed form of the user password
//        /// </summary>
//        public virtual string PasswordHash { get; set; }

//        /// <summary>
//        ///     A random value that should change whenever a users credentials have changed (password changed, login removed)
//        /// </summary>
//        public virtual string SecurityStamp { get; set; }

//        /// <summary>
//        ///     PhoneNumber for the user
//        /// </summary>
//        public virtual string PhoneNumber { get; set; }

//        /// <summary>
//        ///     True if the phone number is confirmed, default is false
//        /// </summary>
//        public virtual bool PhoneNumberConfirmed { get; set; }

//        /// <summary>
//        ///     Is two factor enabled for the user
//        /// </summary>
//        public virtual bool TwoFactorEnabled { get; set; }

//        /// <summary>
//        ///     DateTime in UTC when lockout ends, any time in the past is considered not locked out.
//        /// </summary>
//        public virtual DateTime? LockoutEndDateUtc { get; set; }

//        /// <summary>
//        ///     Is lockout enabled for this user
//        /// </summary>
//        public virtual bool LockoutEnabled { get; set; }

//        /// <summary>
//        ///     Used to record failures for the purposes of lockout
//        /// </summary>
//        public virtual int AccessFailedCount { get; set; }
//    }
//}