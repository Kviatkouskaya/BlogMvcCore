using BlogMvcCore.DomainModel;

namespace BlogMvcCore.Services
{
    public class Authentication
    {
        private readonly IUserAction userActionContext;

        public Authentication(IUserAction userAction)
        {
            userActionContext = userAction;
        }

        public bool CheckUserRegistration(string first, string second, string login,
                                           string password, string repPassword)
        {
            bool stringCheck = CheckStringParams(first, second, login, password, repPassword);
            userActionContext.Register(new User(first, second, login, password));

            return password == repPassword && stringCheck;
        }

        public User CheckIn(string login, string password)
        {

            if (CheckStringParams(login, password) && userActionContext.LoginUser(login, password))
            {
                return userActionContext.FindUser(login);
            }
            return null;
        }

        public void SignOut() => userActionContext.Dispose();

        public bool CheckStringParams(params string[] input)
        {
            foreach (var item in input)
            {
                if (item == string.Empty || string.IsNullOrWhiteSpace(item))
                {
                    return false;
                }
            }
            return true;
        }
    }
}
