using System.Collections.Generic;
using System.Linq;

namespace BlogMvcCore.Storage
{
    public class UserRepository : IUserRepository
    {
        private readonly DbContext DbContext;
        public UserRepository(DbContext context) => DbContext = context;
        public void Dispose() => DbContext.Dispose();

        public DomainModel.UserDomainModel FindUser(string login)
        {
            var user = DbContext.BlogUsers.Where(u => u.Login == login).First();

            return new(user.FirstName, user.SecondName, user.Login, user.Password);
        }

        public List<DomainModel.UserDomainModel> GetUsersList()
        {
            var userDomainList = DbContext.BlogUsers.Select(u => new DomainModel.UserDomainModel(u.FirstName, u.SecondName, u.Login, u.Password))
                                                  .ToList();
            return userDomainList;
        }
    }
}