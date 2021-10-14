using BlogMvcCore.DomainModel;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace BlogMvcCore.Storage
{
    public class Repository : IUserAction  //working with DB connection
    {
        private readonly UserDbContext context;
        public Repository(UserDbContext context)
        {
            this.context = context;
        }

        public DomainModel.User FindUser(string login)
        {
            var user = context.BlogUsers.Where(u => u.Login == login).
                                     FirstOrDefault();
            return new(user.FirstName, user.SecondName, user.Login, user.Password);
        }
        public void AddPost(DomainModel.Post post)
        {
            Post postStorage = new()
            {
                ID = post.ID,
                //Author=post.Author,
                Title = post.Title,
                Text = post.Text,
                Date = post.Date
            };
            context.Attach(postStorage.Author); //input FK before adding post in DB
            context.Posts.Add(postStorage);
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

        public void Register(DomainModel.User newUser)
        {
            var user = new User(newUser.FirstName, newUser.SecondName, newUser.Login, newUser.Password);
            context.BlogUsers.Add(user);
            context.SaveChanges();
        }
        public void Dispose()
        {
            context.Dispose();
        }

        public List<DomainModel.Post> ReturnUserPost(DomainModel.User user)
        {
            var listStorage = context.Posts.Include(u => u.Author).
                                 Where(u => u.Author.Login == user.Login).
                                 OrderByDescending(u => u.Date).
                                 ToList();
            List<DomainModel.Post> postsDomain = new();
            foreach (var item in listStorage)
            {
                DomainModel.Post postDomain = new()
                {
                    ID = item.ID,
                    //Author = item.Author,                   ///convert FK??
                    //Comments = item.Comments,
                    Title = item.Title,
                    Text = item.Text,
                    Date = item.Date
                };
                postsDomain.Add(postDomain);
            }
            return postsDomain;
        }
        public DomainModel.Post FindPost(long postID)
        {
            var postStorage = context.Posts.Find(postID);
            DomainModel.Post postDomain = new()
            {
                ID = postStorage.ID,
                //Author = postStorage.Author,                   ///convert FK??
                //Comments = postStorage.Comments,
                Title = postStorage.Title,
                Text = postStorage.Text,
                Date = postStorage.Date
            };
            return postDomain;
        }
        public void AddComment(DomainModel.Comment comment)
        {
            Comment commentStorage = new()
            {
                ID = comment.ID,
                Author = comment.Author,
                //Post = comment.Post,
                Text = comment.Text,
                Date = comment.Date
            };
            context.Attach(comment.Post);
            context.Comments.Add(commentStorage);
            context.SaveChanges();
        }

        public List<DomainModel.Comment> ReturnPostComment(DomainModel.Post post)
        {

            List<Comment> commentsStorage = context.Comments.Include(p => p.Post).
                                    Where(p => p.Post.ID == post.ID).
                                    OrderByDescending(p => p.Date).
                                    ToList();
            List<DomainModel.Comment> commentsDomain = new();
            foreach (var item in commentsStorage)
            {
                DomainModel.Comment commentDomain = new()
                {
                    ID = item.ID,
                    Author = item.Author,
                    //Post = comment.Post,
                    Text = item.Text,
                    Date = item.Date
                };
                commentsDomain.Add(commentDomain);
            }
            return commentsDomain;
        }

        public List<DomainModel.User> ReturnUsersList()
        {
            List<User> usersStorage = context.BlogUsers.ToList();
            List<DomainModel.User> usersDomain = new();
            foreach (var item in usersStorage)
            {
                DomainModel.User userDomain = new(item.FirstName, item.SecondName, item.Login, item.Password);
                usersDomain.Add(userDomain);
            }
            return usersDomain;
        }
    }
}