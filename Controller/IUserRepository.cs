using System;
using System.Collections.Generic;

namespace BlogMvcCore.Storage
{
    public interface IUserRepository : IDisposable
    {
        List<DomainModel.User> GetUsersList();
        DomainModel.User FindUser(string login);
    }
}
