﻿//using Dominion.Data.MySql;
//using Microsoft.AspNet.Identity;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Security.Claims;
//using System.Threading.Tasks;
//using bcrypt = BCrypt.Net.BCrypt;
//using Dominion.Data.Authorization;

//namespace Dominion.Data.Authorization.User
//{
//    //Todo(apprime): Remove unnecessary interfaces that won't be used by UserStore
//    /// <summary>
//    /// Class that implements the key ASP.NET Identity user store iterfaces
//    /// </summary>
//    public class UserIdentityStore<TUser> : IUserLoginStore<TUser>,
//                                    IUserClaimStore<TUser>,
//                                    IUserRoleStore<TUser>,
//                                    IUserPasswordStore<TUser>,
//                                    IUserSecurityStampStore<TUser>,
//                                    IQueryableUserStore<TUser>,
//                                    IUserEmailStore<TUser>,
//                                    IUserPhoneNumberStore<TUser>,
//                                    IUserTwoFactorStore<TUser, string>,
//                                    IUserLockoutStore<TUser, string>,
//                                    IUserStore<TUser>
//        where TUser : UserIdentity
//    {
//        private UserTable<TUser> userTable;
//        private RoleTable roleTable;
//        private UserRolesTable userRolesTable;
//        private UserClaimsTable userClaimsTable;
//        private UserLoginsTable userLoginsTable;
//        public MySQLDatabase Database { get; private set; }

//        public IQueryable<TUser> Users
//        {
//            get
//            {
//                throw new NotImplementedException();
//            }
//        }

//        private Random _randomNumberGenerator = new Random();

//        /// <summary>
//        /// Default constructor that initializes a new MySQLDatabase
//        /// instance using the Default Connection string
//        /// </summary>
//        public UserIdentityStore()
//        {
//            new UserIdentityStore<TUser>(new MySQLDatabase());
//        }

//        /// <summary>
//        /// Insert a new DominionUser in the UserTable
//        /// </summary>
//        /// <param name="user"></param>
//        /// <returns></returns>
//        public Task CreateAsync(TUser user)
//        {
//            userTable.Insert(user);

//            return Task.FromResult<object>(null);
//        }

//        /// <summary>
//        /// Constructor that takes a MySQLDatabase as argument 
//        /// </summary>
//        /// <param name="database"></param>
//        public UserIdentityStore(MySQLDatabase database)
//        {
//            Database = database;
//            userTable = new UserTable<TUser>(database);
//            roleTable = new RoleTable(database);
//            userRolesTable = new UserRolesTable(database);
//            userClaimsTable = new UserClaimsTable(database);
//            userLoginsTable = new UserLoginsTable(database);
//        }

//        #region STUFF THAT NEEDS TO GO
//        //Interface implementation forces us to have these. 
//        //Probably means we need to not implement those interfaces, but I am not sure what the option is here.
//        public Task<TUser> FindAsync(UserLoginInfo loginInfo)
//        {
//            // Find uses LoginInfo based on third party logins
//            // such as facebook or twitter. 
//            // We do not use that.
//            throw new NotImplementedException();
//        }

//        public Task<TUser> FindByNameAsync(string name)
//        {
//            throw new NotImplementedException();
//        }

//        public Task<TUser> FindByIdAsync(string email)
//        {
//            return Task.FromResult<TUser>(userTable.GetUserByEmail(email));
//        }


//        #endregion

//        /// <summary>
//        /// Returns an TUser instance based on a userId query 
//        /// </summary>
//        /// <param name="userId">The user's Id</param>
//        /// <returns></returns>
//        public Task<TUser> FindByEmail(string userEmail)
//        {
//            TUser user = userTable.GetUserByEmail(userEmail);
//            if (user != null)
//            {
//                return Task.FromResult<TUser>(user);
//            }

//            return Task.FromResult<TUser>(null);
//        }

//        /// <summary>
//        /// Returns an TUser instance based on a userName query 
//        /// </summary>
//        /// <param name="userName">The user's name</param>
//        /// <returns></returns>
//        public Task<List<TUser>> FindByName(string userName)
//        {
//            var users = userTable.GetUsersByName(userName);
//            if (users.Any())
//            {
//                return Task.FromResult<List<TUser>>(users);
//            }

//            return Task.FromResult<List<TUser>>(null);
//        }

//        /// <summary>
//        /// Updates the UsersTable with the TUser instance values
//        /// </summary>
//        /// <param name="user">TUser to be updated</param>
//        /// <returns></returns>
//        public Task UpdateAsync(TUser user)
//        {
//            if (user == null)
//            {
//                throw new ArgumentNullException("user");
//            }

//            userTable.Update(user);

//            return Task.FromResult<object>(null);
//        }

//        public void Dispose()
//        {
//            if (Database != null)
//            {
//                Database.Dispose();
//                Database = null;
//            }
//        }

//        /// <summary>
//        /// Inserts a claim to the UserClaimsTable for the given user
//        /// </summary>
//        /// <param name="user">User to have claim added</param>
//        /// <param name="claim">Claim to be added</param>
//        /// <returns></returns>
//        public Task AddClaimAsync(TUser user, Claim claim)
//        {
//            if (user == null)
//            {
//                throw new ArgumentNullException("user");
//            }

//            if (claim == null)
//            {
//                throw new ArgumentNullException("user");
//            }

//            userClaimsTable.Insert(claim, user.Email);

//            return Task.FromResult<object>(null);
//        }

//        /// <summary>
//        /// Returns all claims for a given user
//        /// </summary>
//        /// <param name="user"></param>
//        /// <returns></returns>
//        public Task<IList<Claim>> GetClaimsAsync(TUser user)
//        {
//            ClaimsIdentity identity = userClaimsTable.FindByUserId(user.Email);

//            return Task.FromResult<IList<Claim>>(identity.Claims.ToList());
//        }

//        /// <summary>
//        /// Removes a claim froma user
//        /// </summary>
//        /// <param name="user">User to have claim removed</param>
//        /// <param name="claim">Claim to be removed</param>
//        /// <returns></returns>
//        public Task RemoveClaimAsync(TUser user, Claim claim)
//        {
//            if (user == null)
//            {
//                throw new ArgumentNullException("user");
//            }

//            if (claim == null)
//            {
//                throw new ArgumentNullException("claim");
//            }

//            userClaimsTable.Delete(user, claim);

//            return Task.FromResult<object>(null);
//        }

//        /// <summary>
//        /// Inserts a Login in the UserLoginsTable for a given User
//        /// </summary>
//        /// <param name="user">User to have login added</param>
//        /// <param name="login">Login to be added</param>
//        /// <returns></returns>
//        public Task AddLoginAsync(TUser user, UserLoginInfo login)
//        {
//            if (user == null)
//            {
//                throw new ArgumentNullException("user");
//            }

//            if (login == null)
//            {
//                throw new ArgumentNullException("login");
//            }

//            userLoginsTable.Insert(user, login);

//            return Task.FromResult<object>(null);
//        }

//        /// <summary>
//        /// Returns an TUser based on the Login info
//        /// </summary>
//        /// <param name="login"></param>
//        /// <returns></returns>
//        public Task<TUser> GetUser(UserLoginInfo login)
//        {
//            var userId = userLoginsTable.FindUserIdByLogin(login);
//            if (userId != null)
//            {
//                TUser user = userTable.GetUserByEmail(userId) as TUser;
//                if (user != null)
//                {
//                    return Task.FromResult<TUser>(user);
//                }
//            }

//            return Task.FromResult<TUser>(null);
//        }

//        /// <summary>
//        /// Returns list of UserLoginInfo for a given TUser
//        /// </summary>
//        /// <param name="user"></param>
//        /// <returns></returns>
//        public Task<IList<UserLoginInfo>> GetLoginsAsync(TUser user)
//        {
//            List<UserLoginInfo> userLogins = new List<UserLoginInfo>();
//            if (user == null)
//            {
//                throw new ArgumentNullException("user");
//            }

//            List<UserLoginInfo> logins = userLoginsTable.FindByUserId(user.Email);
//            if (logins != null)
//            {
//                return Task.FromResult<IList<UserLoginInfo>>(logins);
//            }

//            return Task.FromResult<IList<UserLoginInfo>>(null);
//        }

//        /// <summary>
//        /// Deletes a login from UserLoginsTable for a given TUser
//        /// </summary>
//        /// <param name="user">User to have login removed</param>
//        /// <param name="login">Login to be removed</param>
//        /// <returns></returns>
//        public Task RemoveLoginAsync(TUser user, UserLoginInfo login)
//        {
//            if (user == null)
//            {
//                throw new ArgumentNullException("user");
//            }

//            if (login == null)
//            {
//                throw new ArgumentNullException("login");
//            }

//            userLoginsTable.Delete(user, login);

//            return Task.FromResult<Object>(null);
//        }

//        /// <summary>
//        /// Inserts a entry in the UserRoles table
//        /// </summary>
//        /// <param name="user">User to have role added</param>
//        /// <param name="roleName">Name of the role to be added to user</param>
//        /// <returns></returns>
//        public Task AddToRoleAsync(TUser user, string roleName)
//        {
//            if (user == null)
//            {
//                throw new ArgumentNullException("user");
//            }

//            if (string.IsNullOrEmpty(roleName))
//            {
//                throw new ArgumentException("Argument cannot be null or empty: roleName.");
//            }

//            string roleId = roleTable.GetRoleId(roleName);
//            if (!string.IsNullOrEmpty(roleId))
//            {
//                userRolesTable.Insert(user, roleId);
//            }

//            return Task.FromResult<object>(null);
//        }

//        /// <summary>
//        /// Returns the roles for a given TUser
//        /// </summary>
//        /// <param name="user"></param>
//        /// <returns></returns>
//        public Task<IList<string>> GetRolesAsync(TUser user)
//        {
//            if (user == null)
//            {
//                throw new ArgumentNullException("user");
//            }

//            List<string> roles = userRolesTable.FindByUserId(user.Email);
//            {
//                if (roles != null)
//                {
//                    return Task.FromResult<IList<string>>(roles);
//                }
//            }

//            return Task.FromResult<IList<string>>(null);
//        }

//        /// <summary>
//        /// Verifies if a user is in a role
//        /// </summary>
//        /// <param name="user"></param>
//        /// <param name="role"></param>
//        /// <returns></returns>
//        public Task<bool> IsInRoleAsync(TUser user, string role)
//        {
//            if (user == null)
//            {
//                throw new ArgumentNullException("user");
//            }

//            if (string.IsNullOrEmpty(role))
//            {
//                throw new ArgumentNullException("role");
//            }

//            List<string> roles = userRolesTable.FindByUserId(user.Email);
//            {
//                if (roles != null && roles.Contains(role))
//                {
//                    return Task.FromResult<bool>(true);
//                }
//            }

//            return Task.FromResult<bool>(false);
//        }

//        /// <summary>
//        /// Removes a user from a role
//        /// </summary>
//        /// <param name="user"></param>
//        /// <param name="role"></param>
//        /// <returns></returns>
//        public Task RemoveFromRoleAsync(TUser user, string role)
//        {
//            throw new NotImplementedException();
//        }

//        /// <summary>
//        /// Deletes a user
//        /// </summary>
//        /// <param name="user"></param>
//        /// <returns></returns>
//        public Task DeleteAsync(TUser user)
//        {
//            if (user != null)
//            {
//                userTable.Delete(user);
//            }

//            return Task.FromResult<Object>(null);
//        }

//        /// <summary>
//        /// Returns the PasswordHash for a given TUser
//        /// </summary>
//        /// <param name="user"></param>
//        /// <returns></returns>
//        public Task<string> GetPasswordHashAsync(TUser user)
//        {
//            string passwordHash = userTable.GetPasswordHash(user.Email);

//            return Task.FromResult<string>(passwordHash);
//        }

//        /// <summary>
//        /// Verifies if user has password
//        /// </summary>
//        /// <param name="user"></param>
//        /// <returns></returns>
//        public Task<bool> HasPasswordAsync(TUser user)
//        {
//            var hasPassword = !string.IsNullOrEmpty(userTable.GetPasswordHash(user.Email));

//            return Task.FromResult<bool>(Boolean.Parse(hasPassword.ToString()));
//        }

//        /// <summary>
//        /// Sets the password hash for a given TUser
//        /// </summary>
//        /// <param name="user"></param>
//        /// <param name="passwordHash"></param>
//        /// <returns></returns>
//        public Task SetPasswordHashAsync(TUser user, string passwordHash)
//        {
//            user.PasswordHash = passwordHash;

//            return Task.FromResult<Object>(null);
//        }

//        /// <summary>
//        ///  Set security stamp
//        /// </summary>
//        /// <param name="user"></param>
//        /// <param name="stamp"></param>
//        /// <returns></returns>
//        public Task SetSecurityStampAsync(TUser user, string stamp)
//        {
//            user.SecurityStamp = stamp;

//            return Task.FromResult(0);
//        }

//        /// <summary>
//        /// Get security stamp
//        /// </summary>
//        /// <param name="user"></param>
//        /// <returns></returns>
//        public Task<string> GetSecurityStampAsync(TUser user)
//        {
//            return Task.FromResult(user.SecurityStamp);
//        }

//        /// <summary>
//        /// Set email on user
//        /// </summary>
//        /// <param name="user"></param>
//        /// <param name="email"></param>
//        /// <returns></returns>
//        public Task SetEmailAsync(TUser user, string email)
//        {
//            user.Email = email;
//            userTable.Update(user);

//            return Task.FromResult(0);
//        }

//        /// <summary>
//        /// Get email from user
//        /// </summary>
//        /// <param name="user"></param>
//        /// <returns></returns>
//        public Task<string> GetEmailAsync(TUser user)
//        {
//            return Task.FromResult(user.Email);
//        }

//        /// <summary>
//        /// Get if user email is confirmed
//        /// </summary>
//        /// <param name="user"></param>
//        /// <returns></returns>
//        public Task<bool> GetEmailConfirmedAsync(TUser user)
//        {
//            return Task.FromResult(user.EmailConfirmed);
//        }

//        /// <summary>
//        /// Set when user email is confirmed
//        /// </summary>
//        /// <param name="user"></param>
//        /// <param name="confirmed"></param>
//        /// <returns></returns>
//        public Task SetEmailConfirmedAsync(TUser user, bool confirmed)
//        {
//            user.EmailConfirmed = confirmed;
//            userTable.Update(user);

//            return Task.FromResult(0);
//        }

//        /// <summary>
//        /// Get user by email
//        /// </summary>
//        /// <param name="email"></param>
//        /// <returns></returns>
//        public Task<TUser> FindByEmailAsync(string email)
//        {
//            TUser result = userTable.GetUserByEmail(email);
//            if (result != null)
//            {
//                return Task.FromResult<TUser>(result);
//            }

//            return Task.FromResult<TUser>(null);
//        }

//        /// <summary>
//        /// Set user phone number
//        /// </summary>
//        /// <param name="user"></param>
//        /// <param name="phoneNumber"></param>
//        /// <returns></returns>
//        public Task SetPhoneNumberAsync(TUser user, string phoneNumber)
//        {
//            user.PhoneNumber = phoneNumber;
//            userTable.Update(user);

//            return Task.FromResult(0);
//        }

//        /// <summary>
//        /// Get user phone number
//        /// </summary>
//        /// <param name="user"></param>
//        /// <returns></returns>
//        public Task<string> GetPhoneNumberAsync(TUser user)
//        {
//            return Task.FromResult(user.PhoneNumber);
//        }

//        /// <summary>
//        /// Get if user phone number is confirmed
//        /// </summary>
//        /// <param name="user"></param>
//        /// <returns></returns>
//        public Task<bool> GetPhoneNumberConfirmedAsync(TUser user)
//        {
//            return Task.FromResult(user.PhoneNumberConfirmed);
//        }

//        /// <summary>
//        /// Set phone number if confirmed
//        /// </summary>
//        /// <param name="user"></param>
//        /// <param name="confirmed"></param>
//        /// <returns></returns>
//        public Task SetPhoneNumberConfirmedAsync(TUser user, bool confirmed)
//        {
//            user.PhoneNumberConfirmed = confirmed;
//            userTable.Update(user);

//            return Task.FromResult(0);
//        }

//        /// <summary>
//        /// Set two factor authentication is enabled on the user
//        /// </summary>
//        /// <param name="user"></param>
//        /// <param name="enabled"></param>
//        /// <returns></returns>
//        public Task SetTwoFactorEnabledAsync(TUser user, bool enabled)
//        {
//            user.TwoFactorEnabled = enabled;
//            userTable.Update(user);

//            return Task.FromResult(0);
//        }

//        /// <summary>
//        /// Get if two factor authentication is enabled on the user
//        /// </summary>
//        /// <param name="user"></param>
//        /// <returns></returns>
//        public Task<bool> GetTwoFactorEnabledAsync(TUser user)
//        {
//            return Task.FromResult(user.TwoFactorEnabled);
//        }

//        #region Lockout
//        /// <summary>
//        /// Get user lock out end date
//        /// </summary>
//        /// <param name="user"></param>
//        /// <returns></returns>
//        public Task<DateTimeOffset> GetLockoutEndDateAsync(TUser user)
//        {
//            return
//                Task.FromResult(user.LockoutEndDateUtc.HasValue
//                    ? new DateTimeOffset(DateTime.SpecifyKind(user.LockoutEndDateUtc.Value, DateTimeKind.Utc))
//                    : new DateTimeOffset());
//        }


//        /// <summary>
//        /// Set user lockout end date
//        /// </summary>
//        /// <param name="user"></param>
//        /// <param name="lockoutEnd"></param>
//        /// <returns></returns>
//        public Task SetLockoutEndDateAsync(TUser user, DateTimeOffset lockoutEnd)
//        {
//            user.LockoutEndDateUtc = lockoutEnd.UtcDateTime;
//            userTable.Update(user);

//            return Task.FromResult(0);
//        }

//        /// <summary>
//        /// Increment failed access count
//        /// </summary>
//        /// <param name="user"></param>
//        /// <returns></returns>
//        public Task<int> IncrementAccessFailedCountAsync(TUser user)
//        {
//            user.AccessFailedCount++;
//            userTable.Update(user);

//            return Task.FromResult(user.AccessFailedCount);
//        }

//        /// <summary>
//        /// Reset failed access count
//        /// </summary>
//        /// <param name="user"></param>
//        /// <returns></returns>
//        public Task ResetAccessFailedCountAsync(TUser user)
//        {
//            user.AccessFailedCount = 0;
//            userTable.Update(user);

//            return Task.FromResult(0);
//        }

//        /// <summary>
//        /// Get failed access count
//        /// </summary>
//        /// <param name="user"></param>
//        /// <returns></returns>
//        public Task<int> GetAccessFailedCountAsync(TUser user)
//        {
//            return Task.FromResult(user.AccessFailedCount);
//        }

//        /// <summary>
//        /// Get if lockout is enabled for the user
//        /// </summary>
//        /// <param name="user"></param>
//        /// <returns></returns>
//        public Task<bool> GetLockoutEnabledAsync(TUser user)
//        {
//            return Task.FromResult(user.LockoutEnabled);
//        }

//        /// <summary>
//        /// Set lockout enabled for user
//        /// </summary>
//        /// <param name="user">A User object</param>
//        /// <param name="lockoutLimit">
//        ///     (int) Number of times a user can fail login before lockout occurs.
//        ///     0 or less means it is disabled
//        /// </param>
//        /// <returns></returns>
//        public Task SetLockoutEnabledAsync(TUser user, bool lockoutEnabled)
//        {
//            user.LockoutEnabled = lockoutEnabled;
//            userTable.Update(user);

//            return Task.FromResult(0);
//        }
//        #endregion
//    }
//}