using BlogMvcCore.DomainModel;
using System;
using System.Collections.Generic;

namespace BlogMvcCore.Services
{
    public class PostService
    {
        private readonly IUserAction userActionContext;
        private readonly CommentService commentService;
        public PostService(IUserAction userAction, CommentService commentService)
        {
            userActionContext = userAction;
            this.commentService = commentService;
        }

        private Post GetPost(long postID) => userActionContext.FindPost(postID);

        public List<Post> ReturnPostList()
        {
            return userActionContext.ReturnPostList();
        }

        public void AddPost(string title, string postText, string ownerLogin)
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

        public Post GetPostWithComments(long postID)
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
