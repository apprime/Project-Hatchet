using System;
using Microsoft.AspNet.Identity;
using System.Configuration;
using System.Globalization;

namespace Dominion.Data.Cryptography
{
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
            var salt = BCrypt.Net.BCrypt.GenerateSalt(SaltWorkFactor);
            return BCrypt.Net.BCrypt.HashPassword(password, salt);
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
            if (BCrypt.Net.BCrypt.Verify(candidate, storedPassword))
            {
                //If the workfactor used to hash the password is not the same as the current setting, we need to rehash the candidate with the correct value
                // (We most likely just upgraded servers and decided to increase password strength)
                return (GetHashWorkfactor(storedPassword) == SaltWorkFactor ? PasswordVerificationResult.Success : PasswordVerificationResult.SuccessRehashNeeded);
            }
            else
            {
                return PasswordVerificationResult.Failed;
            }
        }

        /// <summary>
        /// Password hash should be saved to database in the format:
        /// $2a$10$vI8aWBnW3fID.ZQ4/zo1G.q1lRps.9cGLcZEiGDMVr5yUP1KUOYTa -
        /// $[Version]$[Workfactor]$[Salt][PasswordHash]
        /// Version is 2a
        /// Workfactor is always 2 chars, [04-31], defined in code
        /// </summary>
        /// <param name="bcryptHash">The fully hashed password</param>
        /// <returns>Workfactor as integer</returns>
        private static int GetHashWorkfactor(string bcryptHash)
        {
            //Todo(apprime): This is more or less the exact code used in source for bcrypt.
            //However, since we hash all of our passwords the same way, we should be able to ignore the precautions
            //Which are mostly about which version of the algorithm that is used. 

            //int offset;
            //if (bcryptHash[1] != '$')
            //{
            //    char minor = (char)0;
            //    minor = bcryptHash[2];
            //    if (minor != 'a' || bcryptHash[3] != '$')
            //    {
            //        throw new ArgumentException("Invalid salt revision");
            //    }
            //    offset = 4;
            //}
            //else
            //{
            //    offset = 3;
            //}

            //// Extract number of rounds
            //if (bcryptHash[offset + 2] > '$')
            //{
            //    throw new ArgumentException("Missing salt rounds");
            //}

            return Int32.Parse(bcryptHash.Substring(4, 2), NumberFormatInfo.InvariantInfo);
        }
    }
}
