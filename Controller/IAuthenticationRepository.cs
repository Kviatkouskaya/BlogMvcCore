using System;

namespace BlogMvcCore.Storage
{
    public interface IAuthenticationRepository : IDisposable
    {
        bool LoginUser(string login, string password);
        void AddUser(UserEntity user);
    }
}
