using System;
using System.Collections.Generic;

namespace BlogMvcCore.Storage
{
    public interface IUserRepository : IDisposable
    {
        List<DomainModel.UserDomain> GetUsersList();
        DomainModel.UserDomain FindUser(string login);
    }
}
