using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogMvcCore.DomainModel
{
    public interface IAuthenticationAction : IDisposable
    {
        bool LoginUser(string login, string password);
        void Register(User user);
    }
}
