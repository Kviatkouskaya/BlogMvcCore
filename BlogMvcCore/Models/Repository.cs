using Microsoft.EntityFrameworkCore;
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
            context.Attach(post.Author); //input FK before adding post in DB
            context.Posts.Add(post);
            context.SaveChanges();
        }

        public int LoginUser(string login, string password)
        {
            return context.BlogUsers.Where(u => u.Login == login && u.Password == password).
                                     Count();
        }
        public int CheckLoginDuplicate(string login)
        {
            return context.BlogUsers.Where(u => u.Login == login).
                                     Count();
        }

        public void Register(User newUser)
        {
            context.BlogUsers.Add(newUser);
            context.SaveChanges();
        }
        public void Dispose()
        {
            context.Dispose();
        }

        public List<Post> ReturnUserPost(User user)
        {
            return context.Posts.Include(u => u.Author).
                                 Where(u => u.Author.Login == user.Login).
                                 OrderByDescending(u => u.Date).
                                 ToList();
        }
        public Post FindPost(long postID)
        {
            return context.Posts.Find(postID);
        }
        public void AddComment(Comment comment)
        {
            context.Attach(comment.Post);
            context.Comments.Add(comment);
            context.SaveChanges();
        }

        public List<Comment> ReturnPostComment(Post post)
        {
            return context.Comments.Include(p => p.Post).
                                    Where(p => p.Post == post).
                                    OrderByDescending(p => p.Date).
                                    ToList();
        }

        public List<User> ReturnUsersList()
        {
            return context.BlogUsers.ToList();
        }
    }
}
