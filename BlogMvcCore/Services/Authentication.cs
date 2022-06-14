using BlogMvcCore.DomainModel;
using BlogMvcCore.Services;

namespace BlogMvcCore.Services
{
    public class Authentication
    {
        private readonly IAuthenticationAction authenticAction;
        private readonly IUserAction userAction;
        public Authentication(IAuthenticationAction authenticAction, IUserAction userAction)
        {
            this.authenticAction = authenticAction;
            this.userAction = userAction;
        }
        public virtual bool CheckUserRegistration(string first, string second, string login,
                                           string password, string repPassword)
        {
            bool stringCheck = CheckStringParams(first, second, login, password, repPassword);
            authenticAction.Register(new User(first, second, login, password));

            return password == repPassword && stringCheck;
        }

        public virtual User CheckIn(string login, string password)
        {
            if (CheckStringParams(login, password) && authenticAction.LoginUser(login, password))
                return userAction.FindUser(login);

            return null;
        }

        public virtual void SignOut() => authenticAction.Dispose();

        public virtual bool CheckStringParams(params string[] input)
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
