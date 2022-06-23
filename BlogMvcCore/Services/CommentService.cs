using BlogMvcCore.DomainModel;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BlogMvcCore.Services
{
    public class CommentService
    {
        private readonly Storage.ICommentRepository commentRepository;
        private readonly Storage.IPostRepository postRepository;
        public CommentService(Storage.ICommentRepository commentRepository, Storage.IPostRepository postRepository)
        {
            this.commentRepository = commentRepository;
            this.postRepository = postRepository;
        }

        public virtual void AddComment(string commentText, long postID, long parentID, UserDomain user)
        {
            if (string.IsNullOrEmpty(commentText) || string.IsNullOrWhiteSpace(commentText)) return;
            CommentDomain comment = new()
            {
                Post = postRepository.FindPost(postID),
                Author = $"{user.FirstName} {user.SecondName}",
                Parent = parentID,
                Text = commentText,
                Date = DateTime.Now.Date
            };
            comment.Post.Author = user;
            commentRepository.AddComment(comment);
        }

        public virtual void UpdateComment(long commentID, string commentText) => commentRepository.UpdateComment(commentID, commentText);

        public virtual void DeleteComment(long commentID) => commentRepository.DeleteComment(commentID);

        public virtual void FillCommentGen(List<CommentWithLevel> finalList, List<CommentDomain> commentList, int level, long parentID)
        {
            List<CommentWithLevel> commentWithLevels = new();
            List<CommentDomain> childComment = commentList.Where(x => x.Parent == parentID).ToList();
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
