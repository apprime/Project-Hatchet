using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using Dominion.Data.Validation.Regex;

namespace Dominion.Data.Authorization.User
{
    /// <summary>
    /// A UserHandle is a combination of the User's selected name
    /// and a randomly generated Id number in the range 1000-9999
    /// The two combined will uniquely identify a playing user (as a composite PKey)
    /// 
    /// This handle is public and is used to display public information
    /// of a user account
    /// 
    /// Is readonly.
    /// </summary>
    public class UserHandle
    {
        private readonly string m_Name;
        private readonly int m_Id;

        public UserHandle(string name, int id)
        {
            m_Name = name;
            m_Id = id;
        }

        /// <summary>
        /// User Code
        /// This Code is automatically generated upon account generation.
        /// It creates a composite key together with username.
        /// Any one Username + Code is unique
        /// Lock assets to avoid multiple account registrations to occur at once for a username
        /// </summary>
        [Range(1000, 9999)]
        public int Id { get { return m_Id; } }

        /// <summary>
        /// User's name
        /// </summary>
        [RegularExpression(Common.Alphanumeric)]
        public string Name { get { return m_Name; } }
    }
}
