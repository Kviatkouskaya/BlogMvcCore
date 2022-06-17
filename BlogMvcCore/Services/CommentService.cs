using BlogMvcCore.DomainModel;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BlogMvcCore.Services
{
    public class CommentService
    {
        private readonly ICommentAction commentAction;
        private readonly Storage.IPost postAction;
        private readonly Storage.IUser userAction;
        public CommentService(ICommentAction commentAction, Storage.IPost postAction, Storage.IUser userAction)
        {
            this.commentAction = commentAction;
            this.postAction = postAction;
            this.userAction = userAction;
        }

        public virtual void AddComment(string commentText, long postID, long parentID, User user)
        {
            Comment comment = new()
            {
                Post = postAction.FindPost(postID),
                Author = $"{user.FirstName} {user.SecondName}",
                Parent = parentID,
                Text = commentText,
                Date = DateTime.Now.Date
            };
            comment.Post.Author = userAction.FindUser(user.Login);
            commentAction.AddComment(comment);
        }

        public virtual void UpdateComment(long commentID, string commentText) => commentAction.UpdateComment(commentID, commentText);

        public virtual void DeleteComment(long commentID) => commentAction.DeleteComment(commentID);

        public virtual void FillCommentGen(List<CommentWithLevel> finalList, List<Comment> commentList, int level, long parentID)
        {
            List<CommentWithLevel> commentWithLevels = new();
            List<Comment> childComment = commentList.Where(x => x.Parent == parentID).ToList();
            if (childComment.Count != 0)
            {
                foreach (var child in childComment)
                {
                    finalList.Add(new CommentWithLevel { Comment = child, Level = level });
                    var nextLevel = level + 1;
                    FillCommentGen(finalList, commentList, nextLevel, child.ID);
                    var nextCommentGeneration = commentList.Where(x => x.Parent == child.ID).ToList();
                }
            }
        }
    }
}
