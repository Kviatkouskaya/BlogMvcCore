using System;

namespace BlogMvcCore.DomainModel
{
    public interface IAuthenticationAction : IDisposable
    {
        bool LoginUser(string login, string password);
        void Register(User user);
    }
}
