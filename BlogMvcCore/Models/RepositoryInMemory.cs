using System;
using System.Collections.Generic;

namespace BlogMvcCore.Models
{
    public class RepositoryInMemory : IUserAction
    {
        private static readonly List<User> allowedUsers = new() { new User("Admin", "System", "admin", "12345678") };
        private static readonly List<Post> postList = new();
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
            postList.Add(post);
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public List<Post> ReturnUserPost(User user)
        {
            List<Post> userPost = new();
            foreach (var item in postList)
            {
                if (item.Author == user)
                {
                    userPost.Add(item);
                }
            }
            return userPost;
        }
    }
}
