using Dominion.Data.MySql;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using bcrypt = BCrypt.Net.BCrypt;
using Dominion.Data.Authorization;

namespace Dominion.Data.Authorization.User.DominionUser
{
    //Todo(apprime): Remove unnecessary interfaces that won't be used by UserStore
    /// <summary>
    /// Class that implements the key ASP.NET Identity user store iterfaces
    /// </summary>
    public class DominionUserStore<TUser> : IUserLoginStore<DominionUser>,
                                    IUserClaimStore<DominionUser>,
                                    IUserRoleStore<DominionUser>,
                                    IUserPasswordStore<DominionUser>,
                                    IUserSecurityStampStore<DominionUser>,
                                    IQueryableUserStore<DominionUser>,
                                    IUserEmailStore<DominionUser>,
                                    IUserPhoneNumberStore<DominionUser>,
                                    IUserTwoFactorStore<DominionUser, string>,
                                    IUserLockoutStore<DominionUser, string>,
                                    IUserStore<DominionUser>
        where TUser: DominionUser
    {
        private UserTable<DominionUser> userTable;
        private RoleTable roleTable;
        private UserRolesTable userRolesTable;
        private UserClaimsTable userClaimsTable;
        private UserLoginsTable userLoginsTable;
        public MySQLDatabase Database { get; private set; }

        public IQueryable<DominionUser> Users
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        private Random _randomNumberGenerator = new Random();

        /// <summary>
        /// Default constructor that initializes a new MySQLDatabase
        /// instance using the Default Connection string
        /// </summary>
        public DominionUserStore()
        {
            new DominionUserStore<DominionUser>(new MySQLDatabase());
        }

        /// <summary>
        /// Constructor that takes a MySQLDatabase as argument 
        /// </summary>
        /// <param name="database"></param>
        public DominionUserStore(MySQLDatabase database)
        {
            Database = database;
            userTable = new UserTable<DominionUser>(database);
            roleTable = new RoleTable(database);
            userRolesTable = new UserRolesTable(database);
            userClaimsTable = new UserClaimsTable(database);
            userLoginsTable = new UserLoginsTable(database);
        }

        #region STUFF THAT NEEDS TO GO
        //Interface implementation forces us to have these. 
        //Probably means we need to not implement those interfaces, but I am not sure what the option is here.
        public Task<DominionUser> FindAsync(UserLoginInfo loginInfo)
        {
            // Find uses LoginInfo based on third party logins
            // such as facebook or twitter. 
            // We do not use that.
            throw new NotImplementedException();
        }

        public Task<DominionUser> FindByNameAsync(string name)
        {
            throw new NotImplementedException();
        }

        public Task<DominionUser> FindByIdAsync(string email)
        {
            return Task.FromResult<DominionUser>(userTable.GetUserByEmail(email));
        }


        #endregion

        /// <summary>
        /// Insert a new DominionUser in the UserTable
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public Task CreateAsync(DominionUser user)
        {
            
            int newCode;
            bool generatedCode = GenerateCode(user.UserName, out newCode);
            if(generatedCode)
            {
                user.Code= newCode;
                userTable.Insert(user);
            }
            
            return Task.FromResult<object>(null);
        }

        private bool GenerateCode(string username, out int id)
        {
            //Todo(apprime): Convert as much of this as possible to storedProcedure
            //Is dbtable locked? - Wait
            //Todo: Lock DB where Username = "stuff"

            //Get # of usernames like this
            List<DominionUser> users = userTable.GetUsersByName(username);
            
            //If no users are found, just randomly assign a new Id between 1000 and 9999
            if(users == null || !users.Any())
            {
                int randomValue = _randomNumberGenerator.Next(1000, 9999);
                id = randomValue; 
                //Unlock table
                return true;
            }
           
            //If 9000 users, prompt user to pick a different username
            if(users.Count() >= 9000) 
            {
                id = -1; 
                //Unlock table
                return false;
            }

            //Get all taken Id's in a hashSet
            HashSet<int> blacklist = new HashSet<int>(users.Select(user => user.Code));

            //Set up a list of all possible Id's
            List<int> availableIds = Enumerable.Range(1000, 9999).ToList();
            
            //Remove the (subset) taken Id's from all possible.
            availableIds.RemoveAll(availableId => blacklist.Contains(availableId));

            //Select one random number, assign to Id
            int candidate = _randomNumberGenerator.Next(0, availableIds.Count);
            id = availableIds.ElementAt(candidate);
            // TODO: Unlock dbtable
            //Done
            return true;
        }

        /// <summary>
        /// Returns an DominionUser instance based on a userId query 
        /// </summary>
        /// <param name="userId">The user's Id</param>
        /// <returns></returns>
        public Task<DominionUser> FindByEmail(string userEmail)
        {
            DominionUser user = userTable.GetUserByEmail(userEmail);
            if (user != null)
            {
                return Task.FromResult<DominionUser>(user);
            }

            return Task.FromResult<DominionUser>(null);
        }

        /// <summary>
        /// Returns an DominionUser instance based on a userName query 
        /// </summary>
        /// <param name="userName">The user's name</param>
        /// <returns></returns>
        public Task<List<DominionUser>> FindByName(string userName)
        {
            var users = userTable.GetUsersByName(userName);
            if (users.Any())
            {
                return Task.FromResult<List<DominionUser>>(users);
            }

            return Task.FromResult<List<DominionUser>>(null);
        }

        /// <summary>
        /// Updates the UsersTable with the DominionUser instance values
        /// </summary>
        /// <param name="user">DominionUser to be updated</param>
        /// <returns></returns>
        public Task UpdateAsync(DominionUser user)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            userTable.Update(user);

            return Task.FromResult<object>(null);
        }

        public void Dispose()
        {
            if (Database != null)
            {
                Database.Dispose();
                Database = null;
            }
        }

        /// <summary>
        /// Inserts a claim to the UserClaimsTable for the given user
        /// </summary>
        /// <param name="user">User to have claim added</param>
        /// <param name="claim">Claim to be added</param>
        /// <returns></returns>
        public Task AddClaimAsync(DominionUser user, Claim claim)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            if (claim == null)
            {
                throw new ArgumentNullException("user");
            }

            userClaimsTable.Insert(claim, user.Email);

            return Task.FromResult<object>(null);
        }

        /// <summary>
        /// Returns all claims for a given user
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public Task<IList<Claim>> GetClaimsAsync(DominionUser user)
        {
            ClaimsIdentity identity = userClaimsTable.FindByUserId(user.Email);

            return Task.FromResult<IList<Claim>>(identity.Claims.ToList());
        }

        /// <summary>
        /// Removes a claim froma user
        /// </summary>
        /// <param name="user">User to have claim removed</param>
        /// <param name="claim">Claim to be removed</param>
        /// <returns></returns>
        public Task RemoveClaimAsync(DominionUser user, Claim claim)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            if (claim == null)
            {
                throw new ArgumentNullException("claim");
            }

            userClaimsTable.Delete(user.Email, claim);

            return Task.FromResult<object>(null);
        }

        /// <summary>
        /// Inserts a Login in the UserLoginsTable for a given User
        /// </summary>
        /// <param name="user">User to have login added</param>
        /// <param name="login">Login to be added</param>
        /// <returns></returns>
        public Task AddLoginAsync(DominionUser user, UserLoginInfo login)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            if (login == null)
            {
                throw new ArgumentNullException("login");
            }

            userLoginsTable.Insert(user, login);

            return Task.FromResult<object>(null);
        }

        /// <summary>
        /// Returns an DominionUser based on the Login info
        /// </summary>
        /// <param name="login"></param>
        /// <returns></returns>
        public Task<DominionUser> GetUser(UserLoginInfo login)
        {
            var userId = userLoginsTable.FindUserIdByLogin(login);
            if (userId != null)
            {
                DominionUser user = userTable.GetUserByEmail(userId) as DominionUser;
                if (user != null)
                {
                    return Task.FromResult<DominionUser>(user);
                }
            }

            return Task.FromResult<DominionUser>(null);
        }

        /// <summary>
        /// Returns list of UserLoginInfo for a given DominionUser
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public Task<IList<UserLoginInfo>> GetLoginsAsync(DominionUser user)
        {
            List<UserLoginInfo> userLogins = new List<UserLoginInfo>();
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            List<UserLoginInfo> logins = userLoginsTable.FindByUserId(user.Email);
            if (logins != null)
            {
                return Task.FromResult<IList<UserLoginInfo>>(logins);
            }

            return Task.FromResult<IList<UserLoginInfo>>(null);
        }

        /// <summary>
        /// Deletes a login from UserLoginsTable for a given DominionUser
        /// </summary>
        /// <param name="user">User to have login removed</param>
        /// <param name="login">Login to be removed</param>
        /// <returns></returns>
        public Task RemoveLoginAsync(DominionUser user, UserLoginInfo login)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            if (login == null)
            {
                throw new ArgumentNullException("login");
            }

            userLoginsTable.Delete(user, login);

            return Task.FromResult<Object>(null);
        }

        /// <summary>
        /// Inserts a entry in the UserRoles table
        /// </summary>
        /// <param name="user">User to have role added</param>
        /// <param name="roleName">Name of the role to be added to user</param>
        /// <returns></returns>
        public Task AddToRoleAsync(DominionUser user, string roleName)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            if (string.IsNullOrEmpty(roleName))
            {
                throw new ArgumentException("Argument cannot be null or empty: roleName.");
            }

            string roleId = roleTable.GetRoleId(roleName);
            if (!string.IsNullOrEmpty(roleId))
            {
                userRolesTable.Insert(user, roleId);
            }

            return Task.FromResult<object>(null);
        }

        /// <summary>
        /// Returns the roles for a given DominionUser
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public Task<IList<string>> GetRolesAsync(DominionUser user)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            List<string> roles = userRolesTable.FindByUserId(user.Email);
            {
                if (roles != null)
                {
                    return Task.FromResult<IList<string>>(roles);
                }
            }

            return Task.FromResult<IList<string>>(null);
        }

        /// <summary>
        /// Verifies if a user is in a role
        /// </summary>
        /// <param name="user"></param>
        /// <param name="role"></param>
        /// <returns></returns>
        public Task<bool> IsInRoleAsync(DominionUser user, string role)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            if (string.IsNullOrEmpty(role))
            {
                throw new ArgumentNullException("role");
            }

            List<string> roles = userRolesTable.FindByUserId(user.Email);
            {
                if (roles != null && roles.Contains(role))
                {
                    return Task.FromResult<bool>(true);
                }
            }

            return Task.FromResult<bool>(false);
        }

        /// <summary>
        /// Removes a user from a role
        /// </summary>
        /// <param name="user"></param>
        /// <param name="role"></param>
        /// <returns></returns>
        public Task RemoveFromRoleAsync(DominionUser user, string role)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Deletes a user
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public Task DeleteAsync(DominionUser user)
        {
            if (user != null)
            {
                userTable.Delete(user);
            }

            return Task.FromResult<Object>(null);
        }

        /// <summary>
        /// Returns the PasswordHash for a given DominionUser
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public Task<string> GetPasswordHashAsync(DominionUser user)
        {
            string passwordHash = userTable.GetPasswordHash(user.Email);

            return Task.FromResult<string>(passwordHash);
        }

        /// <summary>
        /// Verifies if user has password
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public Task<bool> HasPasswordAsync(DominionUser user)
        {
            var hasPassword = !string.IsNullOrEmpty(userTable.GetPasswordHash(user.Email));

            return Task.FromResult<bool>(Boolean.Parse(hasPassword.ToString()));
        }

        /// <summary>
        /// Sets the password hash for a given DominionUser
        /// </summary>
        /// <param name="user"></param>
        /// <param name="passwordHash"></param>
        /// <returns></returns>
        public Task SetPasswordHashAsync(DominionUser user, string passwordHash)
        {
            user.PasswordHash = passwordHash;

            return Task.FromResult<Object>(null);
        }

        /// <summary>
        ///  Set security stamp
        /// </summary>
        /// <param name="user"></param>
        /// <param name="stamp"></param>
        /// <returns></returns>
        public Task SetSecurityStampAsync(DominionUser user, string stamp)
        {
            user.SecurityStamp = stamp;

            return Task.FromResult(0);
        }

        /// <summary>
        /// Get security stamp
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public Task<string> GetSecurityStampAsync(DominionUser user)
        {
            return Task.FromResult(user.SecurityStamp);
        }

        /// <summary>
        /// Set email on user
        /// </summary>
        /// <param name="user"></param>
        /// <param name="email"></param>
        /// <returns></returns>
        public Task SetEmailAsync(DominionUser user, string email)
        {
            user.Email = email;
            userTable.Update(user);

            return Task.FromResult(0);
        }

        /// <summary>
        /// Get email from user
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public Task<string> GetEmailAsync(DominionUser user)
        {
            return Task.FromResult(user.Email);
        }

        /// <summary>
        /// Get if user email is confirmed
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public Task<bool> GetEmailConfirmedAsync(DominionUser user)
        {
            return Task.FromResult(user.EmailConfirmed);
        }

        /// <summary>
        /// Set when user email is confirmed
        /// </summary>
        /// <param name="user"></param>
        /// <param name="confirmed"></param>
        /// <returns></returns>
        public Task SetEmailConfirmedAsync(DominionUser user, bool confirmed)
        {
            user.EmailConfirmed = confirmed;
            userTable.Update(user);

            return Task.FromResult(0);
        }

        /// <summary>
        /// Get user by email
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public Task<DominionUser> FindByEmailAsync(string email)
        {
            DominionUser result = userTable.GetUserByEmail(email);
            if (result != null)
            {
                return Task.FromResult<DominionUser>(result);
            }

            return Task.FromResult<DominionUser>(null);
        }

        /// <summary>
        /// Set user phone number
        /// </summary>
        /// <param name="user"></param>
        /// <param name="phoneNumber"></param>
        /// <returns></returns>
        public Task SetPhoneNumberAsync(DominionUser user, string phoneNumber)
        {
            user.PhoneNumber = phoneNumber;
            userTable.Update(user);

            return Task.FromResult(0);
        }

        /// <summary>
        /// Get user phone number
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public Task<string> GetPhoneNumberAsync(DominionUser user)
        {
            return Task.FromResult(user.PhoneNumber);
        }

        /// <summary>
        /// Get if user phone number is confirmed
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public Task<bool> GetPhoneNumberConfirmedAsync(DominionUser user)
        {
            return Task.FromResult(user.PhoneNumberConfirmed);
        }

        /// <summary>
        /// Set phone number if confirmed
        /// </summary>
        /// <param name="user"></param>
        /// <param name="confirmed"></param>
        /// <returns></returns>
        public Task SetPhoneNumberConfirmedAsync(DominionUser user, bool confirmed)
        {
            user.PhoneNumberConfirmed = confirmed;
            userTable.Update(user);

            return Task.FromResult(0);
        }

        /// <summary>
        /// Set two factor authentication is enabled on the user
        /// </summary>
        /// <param name="user"></param>
        /// <param name="enabled"></param>
        /// <returns></returns>
        public Task SetTwoFactorEnabledAsync(DominionUser user, bool enabled)
        {
            user.TwoFactorEnabled = enabled;
            userTable.Update(user);

            return Task.FromResult(0);
        }

        /// <summary>
        /// Get if two factor authentication is enabled on the user
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public Task<bool> GetTwoFactorEnabledAsync(DominionUser user)
        {
            return Task.FromResult(user.TwoFactorEnabled);
        }

        #region Lockout
        /// <summary>
        /// Get user lock out end date
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public Task<DateTimeOffset> GetLockoutEndDateAsync(DominionUser user)
        {
            return
                Task.FromResult(user.LockoutEndDateUtc.HasValue
                    ? new DateTimeOffset(DateTime.SpecifyKind(user.LockoutEndDateUtc.Value, DateTimeKind.Utc))
                    : new DateTimeOffset());
        }


        /// <summary>
        /// Set user lockout end date
        /// </summary>
        /// <param name="user"></param>
        /// <param name="lockoutEnd"></param>
        /// <returns></returns>
        public Task SetLockoutEndDateAsync(DominionUser user, DateTimeOffset lockoutEnd)
        {
            user.LockoutEndDateUtc = lockoutEnd.UtcDateTime;
            userTable.Update(user);

            return Task.FromResult(0);
        }

        /// <summary>
        /// Increment failed access count
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public Task<int> IncrementAccessFailedCountAsync(DominionUser user)
        {
            user.AccessFailedCount++;
            userTable.Update(user);

            return Task.FromResult(user.AccessFailedCount);
        }

        /// <summary>
        /// Reset failed access count
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public Task ResetAccessFailedCountAsync(DominionUser user)
        {
            user.AccessFailedCount = 0;
            userTable.Update(user);

            return Task.FromResult(0);
        }

        /// <summary>
        /// Get failed access count
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public Task<int> GetAccessFailedCountAsync(DominionUser user)
        {
            return Task.FromResult(user.AccessFailedCount);
        }

        /// <summary>
        /// Get if lockout is enabled for the user
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public Task<bool> GetLockoutEnabledAsync(DominionUser user)
        {
            return Task.FromResult(user.LockoutEnabled);
        }

        /// <summary>
        /// Set lockout enabled for user
        /// </summary>
        /// <param name="user">A User object</param>
        /// <param name="lockoutLimit">
        ///     (int) Number of times a user can fail login before lockout occurs.
        ///     0 or less means it is disabled
        /// </param>
        /// <returns></returns>
        public Task SetLockoutEnabledAsync(DominionUser user, bool lockoutEnabled)
        {
            user.LockoutEnabled = lockoutEnabled;
            userTable.Update(user);

            return Task.FromResult(0);
        }
        #endregion
    }
}