using BlogMvcCore.DomainModel;

namespace BlogMvcCore.Storage
{
    public interface ICommentRepository
    {
        void AddComment(CommentDomain comment);
        void UpdateComment(long commentID, string text);
        void DeleteComment(long commentID);
    }
}
