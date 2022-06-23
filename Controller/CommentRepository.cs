using BlogMvcCore.DomainModel;
using System.Linq;

namespace BlogMvcCore.Storage
{
    public class CommentRepository : ICommentRepository
    {
        private readonly DbContext DbContext;
        public CommentRepository(DbContext context) => DbContext = context;
        public void Dispose() => DbContext.Dispose();

        public void AddComment(CommentDomain comment)
        {
            DbContext.Comments.Add(new CommentEntity
            {
                ID = comment.ID,
                Parent = comment.Parent,
                Post = DbContext.Posts.Where(x => x.ID == comment.Post.ID)
                                      .SingleOrDefault(),
                Author = comment.Author,
                Text = comment.Text,
                Date = comment.Date
            });
            DbContext.SaveChanges();
        }

        public void UpdateComment(long commentID, string commentText)
        {
            var entityComment = new CommentEntity() { ID = commentID, Text = commentText };
            entityComment.Text = commentText;
            DbContext.Comments.Update(entityComment);
            DbContext.SaveChanges();
        }

        public void DeleteComment(long commentID)
        {
            var entity = new CommentEntity() { ID = commentID };
            DbContext.Entry(entity).State = Microsoft.EntityFrameworkCore.EntityState.Deleted;
            DbContext.SaveChanges();
        }
    }
}
