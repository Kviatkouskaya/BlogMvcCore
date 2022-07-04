using BlogMvcCore.DomainModel;
using System.Collections.Generic;
using System.Linq;

namespace BlogMvcCore.Storage
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext DbContext;
        public UserRepository(AppDbContext context) => DbContext = context;
        public void Dispose() => DbContext.Dispose();

        public UserDomain FindUser(string login)
        {
            var user = DbContext.BlogUsers.Where(u => u.Login == login).First();

            return new(user.FirstName, user.SecondName, user.Login, user.Password);
        }

        public List<UserDomain> GetUsersList()
        {
            var userDomainList = DbContext.BlogUsers.Select(u => new UserDomain(u.FirstName, u.SecondName, u.Login, u.Password))
                                                  .ToList();
            return userDomainList;
        }
    }
}