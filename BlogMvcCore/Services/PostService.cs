using BlogMvcCore.DomainModel;
using System;
using System.Collections.Generic;

namespace BlogMvcCore.Services
{
    public class PostService
    {
        private readonly IUserAction userActionContext;
        public PostService(IUserAction userAction) => userActionContext = userAction;

        public virtual Post GetPost(long postID) => userActionContext.FindPost(postID);

        public virtual List<Post> ReturnPostList()
        {
            return userActionContext.ReturnPostList();
        }

        public virtual void AddPost(string title, string postText, string ownerLogin)
        {
            Post newPost = new()
            {
                Author = userActionContext.FindUser(ownerLogin),
                Title = title,
                Text = postText,
                Date = DateTime.Now.Date
            };
            userActionContext.AddPost(newPost);
        }

        public virtual Post GetPostWithComments(long postID, CommentService commentService)
        {
            var post = GetPost(postID);
            var commentList = userActionContext.ReturnPostComment(post);

            List<CommentWithLevel> commentWithLevels = new();
            commentService.FillCommentGen(commentWithLevels, commentList, 0, default);
            post.Comments = commentWithLevels;

            return post;
        }
    }
}
