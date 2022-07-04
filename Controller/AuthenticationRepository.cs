using System.Linq;

namespace BlogMvcCore.Storage
{
    public class AuthenticationRepository : IAuthenticationRepository
    {
        private readonly AppDbContext DbContext;
        public AuthenticationRepository(AppDbContext dbContext) => DbContext = dbContext;
        public void Dispose() => DbContext.Dispose();

        public UserEntity LoginUser(string login, string password)
        {
            var userCredentials = DbContext.BlogUsers.FirstOrDefault(x => x.Login == login);
            return userCredentials;
        }

        public void AddUser(UserEntity user)
        {
            DbContext.BlogUsers.Add(user);
            DbContext.SaveChanges();
        }
    }
}
