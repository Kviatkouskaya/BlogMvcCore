using BlogMvcCore.DomainModel;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BlogMvcCore.Services
{
    public class CommentService
    {
        private readonly IUserAction userAction;
        private readonly IPostAction postAction;
        public CommentService(IUserAction userAction, IPostAction postAction)
        {
            this.userAction = userAction;
            this.postAction = postAction;
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
            userAction.AddComment(comment);
        }

        public virtual void UpdateComment(long commentID, string commentText) => userAction.UpdateComment(commentID, commentText);

        public virtual void DeleteComment(long commentID) => userAction.DeleteComment(commentID);

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
