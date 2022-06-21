using BlogMvcCore.DomainModel;
using System;
using System.Collections.Generic;

namespace BlogMvcCore.Services
{
    public class PostService
    {
        private readonly Storage.IPostRepository postRepository;
        private readonly Storage.IUserRepository userRepository;
        public PostService(Storage.IPostRepository postRepository, Storage.IUserRepository userRepository)
        {
            this.postRepository = postRepository;
            this.userRepository = userRepository;
        }

        public virtual PostDomain GetPost(long postID) => postRepository.FindPost(postID);

        public virtual List<PostDomain> ReturnPostList()
        {
            return postRepository.GetPostList();
        }

        public virtual void AddPost(string title, string postText, string ownerLogin)
        {
            PostDomain newPost = new()
            {
                Author = userRepository.FindUser(ownerLogin),
                Title = title,
                Text = postText,
                Date = DateTime.Now.Date
            };
            postRepository.AddPost(newPost);
        }

        public virtual void DeletePost(long postID) => postRepository.DeletePost(postID);

        public virtual PostDomain GetPostWithComments(long postID, CommentService commentService)
        {
            var post = GetPost(postID);
            var commentList = postRepository.GetPostComment(post);

            List<CommentWithLevel> commentWithLevels = new();
            commentService.FillCommentGen(commentWithLevels, commentList, 0, default);
            post.Comments = commentWithLevels;

            return post;
        }
    }
}
