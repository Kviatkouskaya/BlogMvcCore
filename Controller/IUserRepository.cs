using System;
using System.Collections.Generic;

namespace BlogMvcCore.Storage
{
    public interface IUserRepository : IDisposable
    {
        List<DomainModel.UserDomainModel> GetUsersList();
        DomainModel.UserDomainModel FindUser(string login);
    }
}
