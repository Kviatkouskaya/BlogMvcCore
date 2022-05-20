using BlogMvcCore.DomainModel;
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

        public void Dispose()
        {
            context.Dispose();
        }

        public List<DomainModel.Post> ReturnUserPost(DomainModel.User user)
        {
            var entityPostsList = context.Posts.Select(p => p)
                                               .Where(p => p.Author.Login == user.Login)
                                               .ToList();

            List<DomainModel.Post> postsDomain = new();
            foreach (var item in entityPostsList)
            {
                if (!postsDomain.Exists(p => p.ID == item.ID))
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
                Parent=comment.Parent,
                Post = entityPost,
                Author = comment.Author,
                Text = comment.Text,
                Date = comment.Date
            });
            context.SaveChanges();
        }

        public List<DomainModel.Comment> ReturnPostComment(DomainModel.Post post)
        {
            var commentsList = context.Comments.Where(c => c.Post.ID == post.ID)
                                               .ToList();

            var result = commentsList.Select(c => new DomainModel.Comment
            {
                Post = new DomainModel.Post
                {
                    ID = post.ID,
                    Author = post.Author,
                    Title =post.Title,
                    Text=post.Text,
                    Date=post.Date,
                    Comments=post.Comments
                },
                ID = c.ID,
                Parent=c.Parent,
                Author = c.Author,
                Text = c.Text,
                Date = c.Date
            }).OrderByDescending(c => c.Date)
              .ToList();
            return result;
        }

        public List<DomainModel.User> ReturnUsersList()
        {
            var userDomainList = context.BlogUsers.Select(u => new DomainModel.User(u.FirstName, u.SecondName, u.Login, u.Password))
                                                  .ToList();
            return userDomainList;
        }
    }
}