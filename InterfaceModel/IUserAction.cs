using System;
using System.Collections.Generic;

namespace BlogMvcCore.DomainModel
{
    public interface IUserAction : IDisposable
    {
        List<User> GetUsersList();
        User FindUser(string login);
    }
}
