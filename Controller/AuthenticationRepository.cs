using System.Linq;

namespace BlogMvcCore.Storage
{
    public class AuthenticationRepository : IAuthenticationRepository
    {
        private readonly AppDbContext DbContext;
        public AuthenticationRepository(AppDbContext dbContext) => DbContext = dbContext;
        public void Dispose() => DbContext.Dispose();

        public bool LoginUser(string login, string password)
        {
            var result = DbContext.BlogUsers.Where(u => u.Login == login && u.Password == password)
                                            .Count();
            return result != 0;
        }

        public void Register(UserEntity newUser)
        {
            var user = new UserEntity(newUser.FirstName, newUser.SecondName, newUser.Login, newUser.Password);
            DbContext.BlogUsers.Add(user);
            DbContext.SaveChanges();
        }
    }
}
