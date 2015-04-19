using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Globalization;
using System.Web;
using Dominion.Data.Authorization.User;
using Dominion.Data.MySql;
using Dominion.Data.Cryptography;
using bcrypt = BCrypt.Net.BCrypt;

namespace Dominion.Data.Authorization.User
{
    public class DominionUserManager : UserManager<DominionUser>
    {
        public DominionUserManager(UserStore<DominionUser> store)
            : base(store)
        {
            
        }

        public static DominionUserManager Create(IdentityFactoryOptions<DominionUserManager> options, IOwinContext context)
        {
            var manager = new DominionUserManager(new UserStore<DominionUser>(context.Get<MySQLDatabase>()));

            manager.PasswordHasher = new BCryptHasher();

            manager.UserValidator = new UserValidator<DominionUser>(manager)
            {
                AllowOnlyAlphanumericUserNames = true,
                RequireUniqueEmail = true,
            };

            // Password requirements are lax. This is because each user should be given the choice of how to manage their passwords. 
            // If it is not important for them to have great security we won't make them. 
            manager.PasswordValidator = new PasswordValidator
            {
                RequiredLength = 6,
                RequireNonLetterOrDigit = false,
                RequireDigit = false,
                RequireLowercase = false,
                RequireUppercase = false
            };

            // Configure user lockout defaults
            manager.UserLockoutEnabledByDefault = true;
            manager.DefaultAccountLockoutTimeSpan = TimeSpan.FromMinutes(5);
            manager.MaxFailedAccessAttemptsBeforeLockout = 5;

            // Register two factor authentication providers. This application uses Phone and Emails as a step of receiving a code for verifying the user
            manager.RegisterTwoFactorProvider("Phone Code", new PhoneNumberTokenProvider<DominionUser> { MessageFormat = "Your security code is {0}" });
            manager.RegisterTwoFactorProvider("Email Code", new EmailTokenProvider<DominionUser> { Subject = "Security Code", BodyFormat = "Your security code is {0}" });

            manager.EmailService = new EmailService();
            manager.SmsService = new SmsService();

            var dataProtectionProvider = options.DataProtectionProvider;
            if (dataProtectionProvider != null)
            {
                manager.UserTokenProvider = new DataProtectorTokenProvider<DominionUser>(dataProtectionProvider.Create("ASP.NET Identity"));
            }
            return manager;
        }

        /// <summary> 
        /// Use Custom approach to verify password 
        /// </summary> 
        public class BCryptHasher : PasswordHasher
        {
            readonly int SaltWorkFactor;

            public BCryptHasher()
            {
                SaltWorkFactor = Int32.Parse(ConfigurationManager.AppSettings["bCryptWorkfactor"]); //Todo(apprime): We do not like calling int.parse on this value every time we log a user in. Web.Config should be cached somehow...
            }
            
            /// <summary>
            /// Takes a cleartext password and hashes it with automatic individual salt
            /// </summary>
            /// <param name="password">(String) of cleartext password</param>
            /// <returns>(String) of hashed password</returns>
            public override string HashPassword(string password)
            {
                var salt = bcrypt.GenerateSalt(SaltWorkFactor);
                return bcrypt.HashPassword(password, salt);
            }

            /// <summary>
            /// Compares a candidate password with a stored password
            /// </summary>
            /// <param name="storedPassword"></param>
            /// <param name="candidate"></param>
            /// <returns></returns>
            public override PasswordVerificationResult VerifyHashedPassword(string storedPassword, string candidate)
            {
                //Todo(apprime): Investigate if Pepper is useful at all. 
                //I.E. should we add a hardcoded string to the actual password to make it harder to use dictionary attacks?
                //This assumes offline attack where the attacker has the database but not the code where the pepper is defined.
                //Pro: A small pepper of 4-6 chars will make dictionary attacks more or less useless without the code
                //Cons: (1)Takes a bit of time for hardly any gain, (2)Needs unit-test to ensure pepper doesn't change suddenly and breaks auth (3) Can never be undone, essentially

                
                if (bcrypt.Verify(candidate, storedPassword))
                {
                    //If the workfactor used to hash the password is not the same as the current setting, we need to rehash the candidate with the correct value
                    // (We most likely just upgraded servers and decided to increase password strength)
                    return bcrypt.GetHashWorkfactor(storedPassword) == SaltWorkFactor ? PasswordVerificationResult.Success : PasswordVerificationResult.SuccessRehashNeeded;                
                }
                else
                {
                    return PasswordVerificationResult.Failed;
                }
            }
        }
    }
}
