using BlogMvcCore.DomainModel;

namespace BlogMvcCore.Services
{
    public class AuthenticationService
    {
        private readonly Storage.IAuthenticationRepository authenticationRepository;
        private readonly Storage.IUserRepository userRepository;
        public AuthenticationService(Storage.IAuthenticationRepository authenticRepository, Storage.IUserRepository userRepository)
        {
            this.authenticationRepository = authenticRepository;
            this.userRepository = userRepository;
        }
        public virtual bool CheckUserRegistration(string first, string second, string login,
                                           string password)
        {
            bool stringCheck = CheckStringParams(first, second, login, password);
            if (stringCheck)
                authenticationRepository.Register(new Storage.UserEntity(first, second, login, password));

            return stringCheck;
        }

        public virtual UserDomain CheckIn(string login, string password)
        {
            if (CheckStringParams(login, password) && authenticationRepository.LoginUser(login, password))
                return userRepository.FindUser(login);

            return null;
        }

        public virtual void SignOut() => authenticationRepository.Dispose();

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
