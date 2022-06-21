namespace BlogMvcCore.Storage
{
    public interface ICommentRepository
    {
        void AddComment(DomainModel.CommentDomain comment);
        void UpdateComment(long commentID, string text);
        void DeleteComment(long commentID);
    }
}
