using BlogMvcCore.DomainModel;
using System.Collections.Generic;
using System.Linq;

namespace BlogMvcCore.Storage
{
    public class CommentRepository : ICommentAction
    {
        private readonly DbContext commentCcontext;
        public CommentRepository(DbContext context) => this.commentCcontext = context;
        public void Dispose() => commentCcontext.Dispose();

        public void AddComment(DomainModel.Comment comment)
        {
            Post entityPost = commentCcontext.Posts.Find(comment.Post.ID);
            commentCcontext.Attach(entityPost);
            commentCcontext.Comments.Add(new Comment
            {
                ID = comment.ID,
                Parent = comment.Parent,
                Post = entityPost,
                Author = comment.Author,
                Text = comment.Text,
                Date = comment.Date
            });
            commentCcontext.SaveChanges();
        }

        public void UpdateComment(long commentID, string commentText)
        {
            var entityComment = commentCcontext.Comments.Find(commentID);
            entityComment.Text = commentText;
            commentCcontext.Comments.Update(entityComment);
            commentCcontext.SaveChanges();
        }

        public void DeleteComment(long commentID)
        {
            var entity = commentCcontext.Comments.FirstOrDefault(x => x.ID == commentID);
            if (entity != null)
                commentCcontext.Entry(entity).State = Microsoft.EntityFrameworkCore.EntityState.Deleted;
            commentCcontext.SaveChanges();
        }
    }
}
