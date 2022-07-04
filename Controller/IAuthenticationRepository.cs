using System;

namespace BlogMvcCore.Storage
{
    public interface IAuthenticationRepository : IDisposable
    {
        UserEntity LoginUser(string login, string password);
        void AddUser(UserEntity user);
    }
}
