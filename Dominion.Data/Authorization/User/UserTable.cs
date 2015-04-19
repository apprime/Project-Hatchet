using System;
using System.Linq;
using System.Collections.Generic;
using Dominion.Data.MySql;

namespace Dominion.Data.Authorization.User
{
    /// <summary>
    /// Class that represents the Users table in the MySQL Database
    /// It gets rows from database and maps them to objects.
    /// Always check methods that call this class for nulls.
    /// </summary>
    public class UserTable<TUser>
        where TUser :DominionUser
    {
        private MySQLDatabase _database;

        /// <summary>
        /// Constructor that takes a MySQLDatabase instance 
        /// </summary>
        /// <param name="database"></param>
        public UserTable(MySQLDatabase database)
        {
            _database = database;
        }

        #region Get UserName, Id
        //Todo(apprime): We don't really need only username or only user id. Remove this?
        /// <summary>
        /// Returns the user's name given a user email
        /// </summary>
        /// <param name="userEmail"></param>
        /// <returns></returns>
        public string GetUserName(string userEmail)
        {
            string commandText = "Select UserName from Users where Email = @email";
            var parameters = new Dictionary<string, object>() { { "@email", userEmail } };

            return _database.GetStrValue(commandText, parameters);
        }

        /// <summary>
        /// Returns a User ID given a user name
        /// </summary>
        /// <param name="userName">The user's name</param>
        /// <returns></returns>
        public string GetUserId(string userEmail)
        {
            string commandText = "Select UserName, Id from Users where Email = @email";
            var parameters = new Dictionary<string, object>() { { "@email", userEmail } };

            return _database.GetStrValue(commandText, parameters);
        }

        /// <summary>
        /// Get a fully qualified user identifier by email
        /// </summary>
        /// <param name="userEmail">(string) Primary key for users</param>
        /// <param name="userData">(out, UserHandle) Object that stores UserName and Id</param>
        /// <returns>(bool) True if a user is found</returns>
        public UserHandle GetUserNameAndId(string userEmail)
        {
            string commandText = "Select UserName, Id from Users where Email = @email";
            var parameters = new Dictionary<string, object>() { { "@email", userEmail } };

            Dictionary<String,String> query = _database.Query(commandText, parameters).FirstOrDefault();

            if(query != null)
            {
                //Todo(apprime): Is this how to treat the incoming query data?
                return new UserHandle
                {
                    //We do not allow null for either of these two columns, no nullcheck is needed
                    UserName = query["UserName"],
                    Id = int.Parse(query["Id"])
                };
            }

            return null;
        }
        #endregion

        #region Get User, Users
        /// <summary>
        /// Returns an TUser given the user's id
        /// </summary>
        /// <param name="userId">The user's id</param>
        /// <returns></returns>
        public TUser GetUserByHandle(UserHandle userHandle)
        {
            string commandText = "Select * from Users where Id = @id and UserName = @username";
            Dictionary<string, object> parameters = new Dictionary<string, object>() 
            { 
                { "@id", userHandle.Id },
                { "@username", userHandle.UserName }
            };

            var rows = _database.Query(commandText, parameters);
            if (!rows.Any()) return null;

            var row = rows[0]; //Assume there is only one user. If not, we have bigger problems than fetching the wrong one
            return MapRowToUser(row);
        }

        /// <summary>
        /// Returns an TUser given the user's id
        /// </summary>
        /// <param name="userId">The user's id</param>
        /// <returns></returns>
        public TUser GetUserByEmail(string email) 
        {
            string commandText = "Select * from Users where Email = @email";
            Dictionary<string, object> parameters = new Dictionary<string, object>() { { "@email", email } };

            var rows = _database.Query(commandText, parameters);
            if (!rows.Any()) return null; //No user was found

            var row = rows[0]; //Assume there is only one user. If not, we have bigger problems than this
            return MapRowToUser(row);
        }

        /// <summary>
        /// Returns a list of TUser instances given a user name
        /// </summary>
        /// <param name="userName">User's name</param>
        /// <returns></returns>
        public List<TUser> GetUsersByName(string userName)
        {
            string commandText = "Select * from Users where UserName = @name";
            Dictionary<string, object> parameters = new Dictionary<string, object>() { { "@name", userName } };

            var rows = _database.Query(commandText, parameters);
            var users = new List<TUser>();
            foreach(var row in rows)
            {
                users.Add(MapRowToUser(row));
            }

            return users;
        }

        /// <summary>
        /// Creates a new user object based on the assumption that 
        /// a row in the database will always contain these properties
        /// </summary>
        /// <param name="row">One row from a user table</param>
        /// <returns>Initialized object of Type TUser</returns>
        private TUser MapRowToUser(Dictionary<string, string> row)
        {
             var user = (TUser)Activator.CreateInstance(typeof(TUser));
            user.Id = int.Parse(row["Id"]);
            user.UserName = row["UserName"];
            user.PasswordHash =row["PasswordHash"];
            user.SecurityStamp = row["SecurityStamp"];
            user.Email = row["Email"];
            user.EmailConfirmed = row["EmailConfirmed"] == "1" ? true : false;
            user.PhoneNumber = row["PhoneNumber"];
            user.PhoneNumberConfirmed = row["PhoneNumberConfirmed"] == "1" ? true : false;
            user.LockoutEnabled = row["LockoutEnabled"] == "1" ? true : false;
            user.TwoFactorEnabled = row["TwoFactorEnabled"] == "1" ? true : false;
            user.LockoutEndDateUtc = DateTime.Parse(row["LockoutEndDateUtc"]);
            user.AccessFailedCount = short.Parse(row["AccessFailedCount"]);
            return user;
                
        }

        /// <summary>
        /// Gets a list of UserHandles for a given user name
        /// </summary>
        /// <param name="userName">User's name</param>
        /// <param name="users">The list by reference</param>
        /// <returns>True if at least one user was found</returns>
        public IEnumerable<UserHandle> GetUserHandlesByName(string userName)
        {
            string commandText = "Select UserName, Id from Users where UserName = @name";
            Dictionary<string, object> parameters = new Dictionary<string, object>() { { "@name", userName } };

            var rows = _database.Query(commandText, parameters);
            IEnumerable<UserHandle> users = rows.Select(row => new UserHandle 
                                                            { 
                                                                Id = int.Parse(row["Id"]), 
                                                                UserName = row["UserName"] 
                                                            } 
                                                        );

            return users;
        }
        #endregion

        #region Password and Securitystamp
        /// <summary>
        /// Return the user's password hash
        /// </summary>
        /// <param name="userId">The user's id</param>
        /// <returns></returns>
        public string GetPasswordHash(string userEmail)
        {
            string commandText = "Select PasswordHash from Users where Email = @email";
            Dictionary<string, object> parameters = new Dictionary<string, object>() { { "@email", userEmail } };

            var passHash = _database.GetStrValue(commandText, parameters);
            if(string.IsNullOrEmpty(passHash))
            {
                return null;
            }

            return passHash;
        }

        /// <summary>
        /// Sets the user's password hash
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="passwordHash"></param>
        /// <returns></returns>
        public int SetPasswordHash(string email, string passwordHash)
        {
            string commandText = "Update Users set PasswordHash = @pwdHash where Email = @email";
            Dictionary<string, object> parameters = new Dictionary<string, object>
                {
                    {"@pwdHash", passwordHash},
                    {"@email", email}
                };

            return _database.Execute(commandText, parameters);
        }

        /// <summary>
        /// Returns the user's security stamp
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public string GetSecurityStamp(string userEmail)
        {
            string commandText = "Select SecurityStamp from Users where Email = @email";
            Dictionary<string, object> parameters = new Dictionary<string, object>() { { "@email", userEmail } };
            var result = _database.GetStrValue(commandText, parameters);

            return result;
        }
        #endregion

        /// <summary>
        /// Inserts a new user in the Users table
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public int Insert(TUser user)
        {
            string commandText = @"Insert into Users (UserName, Id, PasswordHash, SecurityStamp,Email,EmailConfirmed,PhoneNumber,PhoneNumberConfirmed, AccessFailedCount,LockoutEnabled,LockoutEndDateUtc,TwoFactorEnabled)
                values (@name, @id, @pwdHash, @SecStamp,@email,@emailconfirmed,@phonenumber,@phonenumberconfirmed,@accesscount,@lockoutenabled,@lockoutenddate,@twofactorenabled)";
           
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("@name", user.UserName);
            parameters.Add("@id", user.Id);
            parameters.Add("@pwdHash", user.PasswordHash);
            parameters.Add("@SecStamp", user.SecurityStamp);
            parameters.Add("@email", user.Email);
            parameters.Add("@emailconfirmed", user.EmailConfirmed);
            parameters.Add("@phonenumber", user.PhoneNumber);
            parameters.Add("@phonenumberconfirmed", user.PhoneNumberConfirmed);
            parameters.Add("@accesscount", user.AccessFailedCount);
            parameters.Add("@lockoutlimit", user.LockoutLimit);
            parameters.Add("@lockoutenddate", user.LockoutEndDateUtc);
            parameters.Add("@twofactorenabled", user.TwoFactorEnabled);

            return _database.Execute(commandText, parameters);
        }

        #region Delete User
        /// <summary>
        /// Deletes a user from the Users table
        /// </summary>
        /// <param name="userId">The user's id</param>
        /// <returns></returns>
        private int Delete(string userEmail)
        {
            string commandText = "Delete from Users where Email = @userEmail";
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("@userEmail", userEmail);

            return _database.Execute(commandText, parameters);
        }

        /// <summary>
        /// Deletes a user from the Users table
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public int Delete(TUser user)
        {
            return Delete(user.Email);
        }
        #endregion

        /// <summary>
        /// Updates a user in the Users table
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public int Update(TUser user)
        {
            string commandText = @"Update Users set UserName = @userName, PasswordHash = @pswHash, SecurityStamp = @secStamp, 
                Email=@email, EmailConfirmed=@emailconfirmed, PhoneNumber=@phonenumber, PhoneNumberConfirmed=@phonenumberconfirmed,
                AccessFailedCount=@accesscount, LockoutEnabled=@lockoutenabled, LockoutEndDateUtc=@lockoutenddate, TwoFactorEnabled=@twofactorenabled  
                WHERE Id = @userId";

            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("@userName", user.UserName);
            parameters.Add("@pswHash", user.PasswordHash);
            parameters.Add("@secStamp", user.SecurityStamp);
            parameters.Add("@userId", user.Id);
            parameters.Add("@email", user.Email);
            parameters.Add("@emailconfirmed", user.EmailConfirmed);
            parameters.Add("@phonenumber", user.PhoneNumber);
            parameters.Add("@phonenumberconfirmed", user.PhoneNumberConfirmed);
            parameters.Add("@accesscount", user.AccessFailedCount);
            parameters.Add("@lockoutenabled", user.LockoutEnabled);
            parameters.Add("@lockoutenddate", user.LockoutEndDateUtc);
            parameters.Add("@twofactorenabled", user.TwoFactorEnabled);

            return _database.Execute(commandText, parameters);
        }
    }
}
