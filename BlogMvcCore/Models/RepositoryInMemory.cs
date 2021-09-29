using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlogMvcCore.Models
{
    public class RepositoryInMemory : IUserAction
    {
        private static readonly List<User> allowedUsers = new() { new User("Admin", "System", "admin", "12345678") };
        private static List<Post> userPosts = new();
        public bool LoginUser(string login, string password)
        {
            foreach (var item in allowedUsers)
            {
                if (item.Login == login &&
                   item.Password == password)
                {
                    return true;
                }
            }
            return false;
        }
        public bool Register(User user)
        {
            if (user.FirstName != string.Empty && user.SecondName != string.Empty)
            {
                if (user.Login != string.Empty && user.Password != string.Empty)
                {
                    allowedUsers.Add(user);
                    return true;
                }
            }
            return false;
        }
        public User FindUser(string login)
        {
            foreach (var item in allowedUsers)
            {
                if (item.Login == login)
                {
                    return item;
                }
            }
            return null;
        }

        public void AddPost(Post post)
        {
            userPosts.Add(post);
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
