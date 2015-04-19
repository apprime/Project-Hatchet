using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;
using BCrypt.Net;
namespace Dominion.Data.Cryptography
{
    static class BcryptExtensions
    {
        /// <summary>
        /// Password hash should be saved to database in the format:
        /// $2a$10$vI8aWBnW3fID.ZQ4/zo1G.q1lRps.9cGLcZEiGDMVr5yUP1KUOYTa -
        /// $[Version]$[Workfactor]$[Salt][PasswordHash]
        /// Version is 2a
        /// Workfactor is always 2 chars, [04-31], defined in code
        /// </summary>
        /// <param name="bcryptHash">The fully hashed password</param>
        /// <returns>Workfactor as integer</returns>
        private static int GetHashWorkfactor(this BCrypt.Net.BCrypt bcrypt, string bcryptHash)
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
