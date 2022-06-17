using System.Collections.Generic;
using System.Linq;

namespace BlogMvcCore.Storage
{
    public class UserRepository : IUser
    {
        private readonly DbContext DbContext;
        public UserRepository(DbContext context) => DbContext = context;
        public void Dispose() => DbContext.Dispose();

        public DomainModel.User FindUser(string login)
        {
            var user = DbContext.BlogUsers.Where(u => u.Login == login).First();

            return new(user.FirstName, user.SecondName, user.Login, user.Password);
        }

        public List<DomainModel.User> GetUsersList()
        {
            var userDomainList = DbContext.BlogUsers.Select(u => new DomainModel.User(u.FirstName, u.SecondName, u.Login, u.Password))
                                                  .ToList();
            return userDomainList;
        }
    }
}