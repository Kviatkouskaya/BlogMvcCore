using BlogMvcCore.DomainModel;
using System;
using System.Collections.Generic;

namespace BlogMvcCore.Services
{
    public class PostService
    {
        private readonly IPostAction postAction;
        private readonly Storage.IUser userAction;
        public PostService(IPostAction postAction, Storage.IUser userAction)
        {
            this.postAction = postAction;
            this.userAction = userAction;
        }

        public virtual Post GetPost(long postID) => postAction.FindPost(postID);

        public virtual List<Post> ReturnPostList()
        {
            return postAction.GetPostList();
        }

        public virtual void AddPost(string title, string postText, string ownerLogin)
        {
            Post newPost = new()
            {
                Author = userAction.FindUser(ownerLogin),
                Title = title,
                Text = postText,
                Date = DateTime.Now.Date
            };
            postAction.AddPost(newPost);
        }

        public virtual void DeletePost(long postID) => postAction.DeletePost(postID);

        public virtual Post GetPostWithComments(long postID, CommentService commentService)
        {
            var post = GetPost(postID);
            var commentList = postAction.GetPostComment(post);

            List<CommentWithLevel> commentWithLevels = new();
            commentService.FillCommentGen(commentWithLevels, commentList, 0, default);
            post.Comments = commentWithLevels;

            return post;
        }
    }
}
