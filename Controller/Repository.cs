using BlogMvcCore.DomainModel;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace BlogMvcCore.Storage
{
    public class Repository : IUserAction
    {
        private readonly UserDbContext context;
        public Repository(UserDbContext context)
        {
            this.context = context;
        }

        public DomainModel.User FindUser(string login)
        {
            var user = context.BlogUsers.Where(u => u.Login == login).
                                         First();
            return new(user.FirstName, user.SecondName, user.Login, user.Password);
        }

        public void AddPost(DomainModel.Post post)
        {
            User entityAuthor = context.BlogUsers.Where(u => u.Login == post.Author.Login).
                                                  FirstOrDefault();
            Post postStorage = new()
            {
                ID = post.ID,

                Author = entityAuthor,
                Title = post.Title,
                Text = post.Text,
                Date = post.Date
            };
            context.Attach(entityAuthor);
            context.Posts.Add(postStorage);
            context.SaveChanges();
        }

        public bool LoginUser(string login, string password)
        {
            var result = context.BlogUsers.Where(u => u.Login == login && u.Password == password).
                                           Count();
            return result != 0;
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
            var joinEntity = context.Posts.Join(context.Comments,
                                                post => post.Author.Login,
                                                comm => comm.Post.Author.Login,
                                                (posts, comm) => new { Post = posts, Comment = comm }).
                                           Where(postAndComm => postAndComm.Post.Author.Login == user.Login);

            var entityPostsList = joinEntity.Select(p => p).
                                             Where(p => p.Post.Author.Login == user.Login).
                                             ToList();
            List<DomainModel.Post> postsDomain = new();
            foreach (var item in entityPostsList)
            {
                if (!postsDomain.Exists(p => p.ID == item.Post.ID))
                {

                    DomainModel.Post postDomain = new()
                    {
                        ID = item.Post.ID,
                        Author = user,
                        Title = item.Post.Title,
                        Text = item.Post.Text,
                        Date = item.Post.Date,
                    };
                    List<DomainModel.Comment> postComm = new();
                    foreach (var comment in entityPostsList)
                    {
                        DomainModel.Comment commentDomain = new()
                        {
                            ID = comment.Comment.ID,
                            Author = item.Comment.Author,
                            Text = item.Comment.Text,
                            Date = item.Comment.Date
                        };
                        postComm.Add(commentDomain);
                    }
                    postDomain.Comments = postComm;
                    postsDomain.Add(postDomain);
                }
            }
            return postsDomain;
        }

        public DomainModel.Post FindPost(long postID)
        {
            var postStorage = context.Posts.Find(postID);
            DomainModel.Post postDomain = new()
            {
                ID = postStorage.ID,
                Title = postStorage.Title,
                Text = postStorage.Text,
                Date = postStorage.Date
            };
            return postDomain;
        }

        public void AddComment(DomainModel.Comment comment)
        {
            Post entityPost = context.Posts.Find(comment.Post.ID);
            Comment commentStorage = new()
            {
                ID = comment.ID,
                Post = entityPost,
                Author = comment.Author,
                Text = comment.Text,
                Date = comment.Date
            };
            context.Attach(entityPost);
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
                    Text = item.Text,
                    Date = item.Date
                };
                commentsDomain.Add(commentDomain);
            }
            return commentsDomain;
        }

        public List<DomainModel.User> ReturnUsersList()
        {
            List<User> usersStorage = context.BlogUsers.Select(u => u).ToList();
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