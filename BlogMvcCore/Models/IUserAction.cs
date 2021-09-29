using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlogMvcCore.Models
{
    public interface IUserAction : IDisposable
    {
        bool LoginUser(string login, string password);
        bool Register(User user);
        User FindUser(string login);
        public void AddPost(Post post);
    }
}
