//using System;
//using System.Threading.Tasks;
//using System.Collections.Generic;
//using System.Linq;
//using System.Web;
//using Microsoft.AspNet.Identity;
//using Dominion.Data.MySql;
//using Dominion.Data.Authorization.User.DominionUser;

//namespace Dominion.Web.Security
//{
//    public class UserStore<T> : IUserStore<T> where T : DominionUser
//    {
//        public UserStore(MySQLDatabase databaseContext)
//        { }

//        public Task CreateAsync(T user)
//        {
//            throw new NotImplementedException();
//            if (user == null)
//            {
//                throw new ArgumentNullException("user");
//            }

//            //userTable.Insert(user);

//            return Task.FromResult<object>(null);
//        }

//        public Task DeleteAsync(T user)
//        {
//            throw new NotImplementedException();
//        }

//        public Task<T> FindByIdAsync(string userId)
//        {
//            throw new NotImplementedException();
//        }

//        public Task<T> FindByNameAsync(string userName)
//        {
//            throw new NotImplementedException();
//        }

//        public Task UpdateAsync(T user)
//        {
//            throw new NotImplementedException();
//        }

//        public void Dispose()
//        {
//            throw new NotImplementedException();
//        }
//    }
//}