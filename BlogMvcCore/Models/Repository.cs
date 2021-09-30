using System.Collections.Generic;
using System.Linq;

namespace BlogMvcCore.Models
{
    public class Repository : IUserAction  //working with DB connection
    {
        private readonly UserDbContext context;
        public Repository(UserDbContext context)
        {
            this.context = context;
        }

        public User FindUser(string login)
        {
            return context.BlogUsers.Where(u => u.Login == login).
                                     FirstOrDefault();
        }
        public void AddPost(Post post)
        {
            context.Posts.Add(post);
            context.SaveChanges();
        }

        public bool LoginUser(string login, string password)
        {
            int result = context.BlogUsers.Where(u => u.Login == login
                                                && u.Password == password).
                                           Count();
            return result > 0;
        }

        public bool Register(User newUser)
        {
            int count = context.BlogUsers.Where(u => u.Login == newUser.Login).
                                          Count();
            if (count == 0)
            {
                context.BlogUsers.Add(newUser);
                context.SaveChanges();
                return true;
            }
            return false;
        }
        public void Dispose()
        {
            context.Dispose();
        }

        public List<Post> ReturnUserPost(User user)
        {
            List<Post> userPost = context.Posts.Where(u => u.Author.Login == user.Login).
                                                OrderByDescending(u => u.Date).
                                                ToList();
            return userPost;
        }
    }
}
