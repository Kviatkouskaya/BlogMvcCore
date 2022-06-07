using BlogMvcCore.DomainModel;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BlogMvcCore.Services
{
    public class CommentService
    {
        private readonly IUserAction userActionContext;
        public CommentService(IUserAction userAction) => userActionContext = userAction;

        public virtual void AddComment(string commentText, long postID, long parentID, User user)
        {
            Comment comment = new()
            {
                Post = userActionContext.FindPost(postID),
                Author = $"{user.FirstName} {user.SecondName}",
                Parent = parentID,
                Text = commentText,
                Date = DateTime.Now.Date
            };
            comment.Post.Author = userActionContext.FindUser(user.Login);
            userActionContext.AddComment(comment);
        }

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
