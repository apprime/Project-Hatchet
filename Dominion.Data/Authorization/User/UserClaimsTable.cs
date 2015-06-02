using System.Collections.Generic;
using System.Security.Claims;
using Dominion.Data.MySql;

namespace Dominion.Data.Authorization.User
{
    /// <summary>
    /// Class that represents the UserClaims table in the MySQL Database
    /// </summary>
    public class UserClaimsTable
    {
        private MySQLDatabase _database;

        /// <summary>
        /// Constructor that takes a MySQLDatabase instance 
        /// </summary>
        /// <param name="database"></param>
        public UserClaimsTable(MySQLDatabase database)
        {
            _database = database;
        }

        /// <summary>
        /// TODO(apprime): Write words here.
        /// </summary>
        /// <param name="userEmail"></param>
        /// <returns></returns>
        public ClaimsIdentity FindByUserEmail(string userEmail)
        {
            ClaimsIdentity claims = new ClaimsIdentity();
            string commandText = "Select * from UserClaims where UserEmail = @useremail";
            Dictionary<string, object> parameters = new Dictionary<string, object>() { { "@useremail", userEmail } };

            var rows = _database.Query(commandText, parameters);
            foreach (var row in rows)
            {
                Claim claim = new Claim(row["ClaimType"], row["ClaimValue"]);
                claims.AddClaim(claim);
            }

            return claims;
        }

        /// <summary>
        /// Returns a ClaimsIdentity instance given a userId
        /// </summary>
        /// <param name="userId">The user's id</param>
        /// <returns></returns>
        public ClaimsIdentity FindByUserId(string id)
        {
            ClaimsIdentity claims = new ClaimsIdentity();
            string commandText = "Select * from UserClaims where Id = @id";
            Dictionary<string, object> parameters = new Dictionary<string, object>() 
            { 
                { "@username", id }, 
            };

            var rows = _database.Query(commandText, parameters);
            foreach (var row in rows)
            {
                Claim claim = new Claim(row["ClaimType"], row["ClaimValue"]);
                claims.AddClaim(claim);
            }

            return claims;
        }

        /// <summary>
        /// Deletes all claims from a user given a userId
        /// </summary>
        /// <param name="userId">The user's id</param>
        /// <returns></returns>
        public int Delete(UserHandle user)
        {
            string commandText = "Delete from UserClaims where UserName = @username and Id = @id";
            Dictionary<string, object> parameters = new Dictionary<string, object>()           
            { 
                { "@username", user.Id }, 
                {"@id", user.Name }
            };

            return _database.Execute(commandText, parameters);
        }

        /// <summary>
        /// Inserts a new claim in UserClaims table
        /// </summary>
        /// <param name="userClaim">User's claim to be added</param>
        /// <param name="userId">User's id</param>
        /// <returns></returns>
        public int Insert(Claim userClaim, string id)
        {
            string commandText = "Insert into UserClaims (ClaimValue, ClaimType, UserId) values (@value, @type, @userId)";
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("id", id);
            parameters.Add("value", userClaim.Value);
            parameters.Add("type", userClaim.Type);

            return _database.Execute(commandText, parameters);
        }

        /// <summary>
        /// Deletes a claim from a user 
        /// </summary>
        /// <param name="user">The user to have a claim deleted</param>
        /// <param name="claim">A claim to be deleted from user</param>
        /// <returns></returns>
        public int Delete(string id, Claim claim)
        {
            string commandText = "Delete from UserClaims where UserId = @userId and @ClaimValue = @value and ClaimType = @type";
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("id", id);
            parameters.Add("value", claim.Value);
            parameters.Add("type", claim.Type);

            return _database.Execute(commandText, parameters);
        }
    }
}
