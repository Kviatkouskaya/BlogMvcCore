using BlogMvcCore.DomainModel;
using System;
using System.Collections.Generic;

namespace BlogMvcCore.Storage
{
    public interface IUserRepository : IDisposable
    {
        List<UserDomain> GetUsersList();
        UserDomain FindUser(string login);
    }
}
