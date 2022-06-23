using BlogMvcCore.DomainModel;
using System.Collections.Generic;
using System.Linq;

namespace BlogMvcCore.Storage
{
    public class PostRepository : IPostRepository
    {
        private readonly DbContext DbContext;
        public PostRepository(DbContext context) => DbContext = context;
        public void Dispose() => DbContext.Dispose();

        public void AddPost(PostDomain post)
        {
            UserEntity entityAuthor = DbContext.BlogUsers.Where(u => u.Login == post.Author.Login).
                                                  First();
            DbContext.Attach(entityAuthor);
            DbContext.Posts.Add(new PostEntity()
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
            var entityPost = new PostEntity() { ID = postID };
            if (entityPost != null)
                DbContext.Entry(entityPost).State = Microsoft.EntityFrameworkCore.EntityState.Deleted;
            DbContext.SaveChanges();
        }

        public List<PostDomain> GetUserPost(UserDomain user)
        {
            var entityPostsList = DbContext.Posts.Where(p => p.Author.Login == user.Login)
                                               .ToList();

            List<PostDomain> postsDomain = new();
            foreach (var item in entityPostsList)
            {
                PostDomain postDomain = new()
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

        public PostDomain FindPost(long postID)
        {
            var postStorage = DbContext.Posts.Find(postID);

            return new PostDomain()
            {
                ID = postStorage.ID,
                Title = postStorage.Title,
                Text = postStorage.Text,
                Date = postStorage.Date
            }; ;
        }

        public List<PostDomain> GetPostList()
        {
            var entityPostsList = DbContext.Posts.ToList().OrderByDescending(p => p.Date);

            List<PostDomain> postList = new();
            foreach (var item in entityPostsList)
            {
                PostDomain postDomain = new()
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

        public List<CommentDomain> GetPostComment(PostDomain post)
        {
            var entityComments = DbContext.Comments.Where(c => c.Post.ID == post.ID).ToList();

            var commentList = entityComments.Select(c => new CommentDomain
            {
                Post = new PostDomain
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
