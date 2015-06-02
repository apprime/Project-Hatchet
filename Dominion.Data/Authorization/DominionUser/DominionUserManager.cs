using Dominion.Data.Authorization.User.Store;
using Dominion.Data.Cryptography;
using Dominion.Data.MySql;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using System;


namespace Dominion.Data.Authorization.User.DominionUser
{
    public class DominionUserManager : UserManager<DominionUser>
    {
        public DominionUserManager(IUserStore<DominionUser> store)
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
            }
            return manager;
        }

    }
}
