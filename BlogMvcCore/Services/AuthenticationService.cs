using BlogMvcCore.DomainModel;
using BCrypt.Net;

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

        public virtual bool AddUser(string first, string second, string login, string password)
        {
            bool stringCheck = CheckStringParams(first, second, login, password);
            if (stringCheck)
            {
                string salt = BCrypt.Net.BCrypt.GenerateSalt();
                string hashedPassword = BCrypt.Net.BCrypt.HashPassword(password, salt);
                authenticationRepository.AddUser(new Storage.UserEntity(first, second, login, hashedPassword, salt));
            }
            return stringCheck;
        }

        public virtual UserDomain CheckIn(string login, string password)
        {
            if (CheckStringParams(login, password))
            {
                Storage.UserEntity userEntity = authenticationRepository.LoginUser(login, password);

                bool verifiedPassword = BCrypt.Net.BCrypt.Verify(password + userEntity.Salt, userEntity.Password);
                if (login == userEntity.Login && verifiedPassword)
                    return userRepository.FindUser(login);
            }

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
