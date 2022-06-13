using BlogMvcCore.DomainModel;
using System.Collections.Generic;
using System.Linq;

namespace BlogMvcCore.Storage
{
    public class Repository : IUserAction
    {
        private readonly UserDbContext context;
        public Repository(UserDbContext context) => this.context = context;

        public DomainModel.User FindUser(string login)
        {
            var user = context.BlogUsers.Where(u => u.Login == login).First();

            return new(user.FirstName, user.SecondName, user.Login, user.Password);
        }

        public void AddPost(DomainModel.Post post)
        {
            User entityAuthor = context.BlogUsers.Where(u => u.Login == post.Author.Login).
                                                  First();
            context.Attach(entityAuthor);
            context.Posts.Add(new Post()
            {
                ID = post.ID,
                Author = entityAuthor,
                Title = post.Title,
                Text = post.Text,
                Date = post.Date
            });
            context.SaveChanges();
        }

        public void DeletePost(long postID)
        {
            var deletingPost = context.Posts.FirstOrDefault(x => x.ID == postID);
            if (deletingPost != null)
                context.Entry(deletingPost).State = Microsoft.EntityFrameworkCore.EntityState.Deleted;
            context.SaveChanges();
        }

        public bool LoginUser(string login, string password)
        {
            var result = context.BlogUsers.Where(u => u.Login == login && u.Password == password)
                                          .Count();
            return result != 0;
        }

        public void Register(DomainModel.User newUser)
        {
            var user = new User(newUser.FirstName, newUser.SecondName, newUser.Login, newUser.Password);
            context.BlogUsers.Add(user);
            context.SaveChanges();
        }

        public void Dispose() => context.Dispose();

        public List<DomainModel.Post> GetUserPost(DomainModel.User user)
        {
            var entityPostsList = context.Posts.Where(p => p.Author.Login == user.Login)
                                               .ToList();

            List<DomainModel.Post> postsDomain = new();
            foreach (var item in entityPostsList)
            {
                DomainModel.Post postDomain = new()
                {
                    ID = item.ID,
                    Author = user,
                    Title = item.Title,
                    Text = item.Text,
                    Date = item.Date
                };
                postsDomain.Add(postDomain);
            }

            return postsDomain.OrderByDescending(p => p.Date).ToList();
        }

        public DomainModel.Post FindPost(long postID)
        {
            var postStorage = context.Posts.Find(postID);

            return new DomainModel.Post()
            {
                ID = postStorage.ID,
                Title = postStorage.Title,
                Text = postStorage.Text,
                Date = postStorage.Date
            }; ;
        }

        public void AddComment(DomainModel.Comment comment)
        {
            Post entityPost = context.Posts.Find(comment.Post.ID);
            context.Attach(entityPost);
            context.Comments.Add(new Comment
            {
                ID = comment.ID,
                Parent = comment.Parent,
                Post = entityPost,
                Author = comment.Author,
                Text = comment.Text,
                Date = comment.Date
            });
            context.SaveChanges();
        }

        public void UpdateComment(long commentID, string commentText)
        {
            var entityComment = context.Comments.Find(commentID);
            entityComment.Text = commentText;
            context.Comments.Update(entityComment);
            context.SaveChanges();
        }

        public void DeleteComment(long commentID)
        {
            var entity = context.Comments.FirstOrDefault(x => x.ID == commentID);
            if (entity != null)
                context.Entry(entity).State = Microsoft.EntityFrameworkCore.EntityState.Deleted;
            context.SaveChanges();
        }

        public List<DomainModel.Comment> GetPostComment(DomainModel.Post post)
        {
            var entityComments = context.Comments.Where(c => c.Post.ID == post.ID).ToList();

            var commentList = entityComments.Select(c => new DomainModel.Comment
            {
                Post = new DomainModel.Post
                {
                    ID = post.ID,
                    Author = post.Author,
                    Title = post.Title,
                    Text = post.Text,
                    Date = post.Date,
                    Comments = post.Comments
                },
                ID = c.ID,
                Parent = c.Parent,
                Author = c.Author,
                Text = c.Text,
                Date = c.Date
            }).OrderByDescending(c => c.Date)
              .ToList();

            return commentList;
        }

        public List<DomainModel.User> GetUsersList()
        {
            var userDomainList = context.BlogUsers.Select(u => new DomainModel.User(u.FirstName, u.SecondName, u.Login, u.Password))
                                                  .ToList();
            return userDomainList;
        }

        public List<DomainModel.Post> GetPostList()
        {
            var entityPostsList = context.Posts.ToList().OrderByDescending(p => p.Date);

            List<DomainModel.Post> postList = new();
            foreach (var item in entityPostsList)
            {
                DomainModel.Post postDomain = new()
                {
                    ID = item.ID,
                    Title = item.Title,
                    Text = item.Text,
                    Date = item.Date
                };
                postList.Add(postDomain);
            }

            return postList;
        }
    }
}