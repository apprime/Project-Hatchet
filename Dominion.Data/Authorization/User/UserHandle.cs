using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dominion.Data.Authorization.User
{
    /// <summary>
    /// A UserHandle is a combination of the User's selected name
    /// and a randomly generated Id number in the range 1000-9999
    /// The two combined will uniquely identify a user (as a composite PKey)
    /// </summary>
    public class UserHandle
    {
        public string UserName { get; set; }
        public int Id { get; set; }
    }
}
