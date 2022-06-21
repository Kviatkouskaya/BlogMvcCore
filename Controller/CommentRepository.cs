using BlogMvcCore.DomainModel;
using System.Linq;

namespace BlogMvcCore.Storage
{
    public class CommentRepository : ICommentRepository
    {
        private readonly DbContext DbContext;
        public CommentRepository(DbContext context) => DbContext = context;
        public void Dispose() => DbContext.Dispose();

        public void AddComment(DomainModel.Comment comment)
        {
            Post entityPost = DbContext.Posts.Find(comment.Post.ID);
            DbContext.Attach(entityPost);
            DbContext.Comments.Add(new Comment
            {
                ID = comment.ID,
                Parent = comment.Parent,
                Post = entityPost,
                Author = comment.Author,
                Text = comment.Text,
                Date = comment.Date
            });
            DbContext.SaveChanges();
        }

        public void UpdateComment(long commentID, string commentText)
        {
            var entityComment = DbContext.Comments.Find(commentID);
            entityComment.Text = commentText;
            DbContext.Comments.Update(entityComment);
            DbContext.SaveChanges();
        }

        public void DeleteComment(long commentID)
        {
            var entity = DbContext.Comments.FirstOrDefault(x => x.ID == commentID);
            if (entity != null)
                DbContext.Entry(entity).State = Microsoft.EntityFrameworkCore.EntityState.Deleted;
            DbContext.SaveChanges();
        }
    }
}
