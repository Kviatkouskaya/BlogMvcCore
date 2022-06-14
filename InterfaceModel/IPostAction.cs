using System;
using System.Collections.Generic;

namespace BlogMvcCore.DomainModel
{
    public interface IPostAction : IDisposable
    {
        void AddPost(Post post);
        void DeletePost(long postID);
        List<Post> GetUserPost(User user);
        List<Comment> GetPostComment(Post post);
        Post FindPost(long postID);
        List<Post> GetPostList();
    }
}
