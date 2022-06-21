using System.Collections.Generic;
using System.Linq;

namespace BlogMvcCore.Storage
{
    public class UserRepository : IUserRepository
    {
        private readonly DbContext DbContext;
        public UserRepository(DbContext context) => DbContext = context;
        public void Dispose() => DbContext.Dispose();

        public DomainModel.UserDomain FindUser(string login)
        {
            var user = DbContext.BlogUsers.Where(u => u.Login == login).First();

            return new(user.FirstName, user.SecondName, user.Login, user.Password);
        }

        public List<DomainModel.UserDomain> GetUsersList()
        {
            var userDomainList = DbContext.BlogUsers.Select(u => new DomainModel.UserDomain(u.FirstName, u.SecondName, u.Login, u.Password))
                                                  .ToList();
            return userDomainList;
        }
    }
}