using System.Linq;

namespace BlogMvcCore.Storage
{
    public class AuthenticationRepository : IAuthenticationRepository
    {
        private readonly DbContext DbContext;
        public AuthenticationRepository(DbContext dbContext) => DbContext = dbContext;
        public void Dispose() => DbContext.Dispose();

        public bool LoginUser(string login, string password)
        {
            var result = DbContext.BlogUsers.Where(u => u.Login == login && u.Password == password)
                                          .Count();
            return result != 0;
        }

        public void Register(User newUser)
        {
            var user = new User(newUser.FirstName, newUser.SecondName, newUser.Login, newUser.Password);
            DbContext.BlogUsers.Add(user);
            DbContext.SaveChanges();
        }
    }
}
