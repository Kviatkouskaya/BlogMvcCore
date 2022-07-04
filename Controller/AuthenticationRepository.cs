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
            var userCredentials = DbContext.BlogUsers.Select(x => new UserEntity(null, null, x.Login, x.Password, x.Salt))
                                                     .FirstOrDefault(x => x.Login == login);
            //var result = DbContext.BlogUsers.Where(u => u.Login == login && u.Password == password)
            //.Count();
            return userCredentials;
        }

        public void AddUser(UserEntity user)
        {
            // var user = new UserEntity(newUser.FirstName, newUser.SecondName, newUser.Login, newUser.Password, newUser.Salt);
            DbContext.BlogUsers.Add(user);
            DbContext.SaveChanges();
        }
    }
}
