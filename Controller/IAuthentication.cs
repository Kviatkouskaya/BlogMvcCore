using System;

namespace BlogMvcCore.Storage
{
    public interface IAuthentication : IDisposable
    {
        bool LoginUser(string login, string password);
        void Register(User user);
    }
}
