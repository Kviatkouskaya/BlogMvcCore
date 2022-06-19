using BlogMvcCore.DomainModel;

namespace BlogMvcCore.Services
{
    public class Authentication
    {
        private readonly Storage.IAuthentication authenticAction;
        private readonly Storage.IUser userAction;
        public Authentication(Storage.IAuthentication authenticAction, Storage.IUser userAction)
        {
            this.authenticAction = authenticAction;
            this.userAction = userAction;
        }
        public virtual bool CheckUserRegistration(string first, string second, string login,
                                           string password, string repPassword)
        {
            bool stringCheck = CheckStringParams(first, second, login, password, repPassword);
            authenticAction.Register(new Storage.User(first, second, login, password));

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
