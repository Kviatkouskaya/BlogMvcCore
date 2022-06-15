using BlogMvcCore.DomainModel;
using System.Collections.Generic;
using System.Linq;

namespace BlogMvcCore.Storage
{
    public class PostRepository : IPostAction
    {
        private readonly DbContext DbContext;
        public PostRepository(DbContext context) => this.DbContext = context;
        public void Dispose() => DbContext.Dispose();

        public void AddPost(DomainModel.Post post)
        {
            User entityAuthor = DbContext.BlogUsers.Where(u => u.Login == post.Author.Login).
                                                  First();
            DbContext.Attach(entityAuthor);
            DbContext.Posts.Add(new Post()
            {
                ID = post.ID,
                Author = entityAuthor,
                Title = post.Title,
                Text = post.Text,
                Date = post.Date
            });
            DbContext.SaveChanges();
        }

        public void DeletePost(long postID)
        {
            var deletingPost = DbContext.Posts.FirstOrDefault(x => x.ID == postID);
            if (deletingPost != null)
                DbContext.Entry(deletingPost).State = Microsoft.EntityFrameworkCore.EntityState.Deleted;
            DbContext.SaveChanges();
        }

        public List<DomainModel.Post> GetUserPost(DomainModel.User user)
        {
            var entityPostsList = DbContext.Posts.Where(p => p.Author.Login == user.Login)
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
            var postStorage = DbContext.Posts.Find(postID);

            return new DomainModel.Post()
            {
                ID = postStorage.ID,
                Title = postStorage.Title,
                Text = postStorage.Text,
                Date = postStorage.Date
            }; ;
        }

        public List<DomainModel.Post> GetPostList()
        {
            var entityPostsList = DbContext.Posts.ToList().OrderByDescending(p => p.Date);

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

        public List<DomainModel.Comment> GetPostComment(DomainModel.Post post)
        {
            var entityComments = DbContext.Comments.Where(c => c.Post.ID == post.ID).ToList();

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
            }).OrderByDescending(c => c.Date).ToList();

            return commentList;
        }
    }
}
