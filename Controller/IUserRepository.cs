using System;
using System.Collections.Generic;
using BlogMvcCore.DomainModel;

namespace BlogMvcCore.Storage
{
    public interface IUserRepository : IDisposable
    {
        List<UserDomain> GetUsersList();
        UserDomain FindUser(string login);
    }
}
