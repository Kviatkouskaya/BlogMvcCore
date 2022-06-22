using System;
using System.Collections.Generic;
using BlogMvcCore.DomainModel;

namespace BlogMvcCore.Storage
{
    public interface IPostRepository : IDisposable
    {
        void AddPost(PostDomain post);
        void DeletePost(long postID);
        List<PostDomain> GetUserPost(UserDomain user);
        List<CommentDomain> GetPostComment(PostDomain post);
        PostDomain FindPost(long postID);
        List<PostDomain> GetPostList();
    }
}
