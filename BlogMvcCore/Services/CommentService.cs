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
        private readonly Storage.IUserRepository userRepository;
        public CommentService(Storage.ICommentRepository commentRepository, Storage.IPostRepository postRepository, Storage.IUserRepository userRepository)
        {
            this.commentRepository = commentRepository;
            this.postRepository = postRepository;
            this.userRepository = userRepository;
        }

        public virtual void AddComment(string commentText, long postID, long parentID, UserDomainModel user)
        {
            CommentDomainModel comment = new()
            {
                Post = postRepository.FindPost(postID),
                Author = $"{user.FirstName} {user.SecondName}",
                Parent = parentID,
                Text = commentText,
                Date = DateTime.Now.Date
            };
            comment.Post.Author = userRepository.FindUser(user.Login);
            commentRepository.AddComment(comment);
        }

        public virtual void UpdateComment(long commentID, string commentText) => commentRepository.UpdateComment(commentID, commentText);

        public virtual void DeleteComment(long commentID) => commentRepository.DeleteComment(commentID);

        public virtual void FillCommentGen(List<CommentWithLevel> finalList, List<CommentDomainModel> commentList, int level, long parentID)
        {
            List<CommentWithLevel> commentWithLevels = new();
            List<CommentDomainModel> childComment = commentList.Where(x => x.Parent == parentID).ToList();
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
