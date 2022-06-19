using System;
using System.Collections.Generic;
using BlogMvcCore.DomainModel;

namespace BlogMvcCore.Storage
{
    public interface IPost : IDisposable
    {
        void AddPost(DomainModel.Post post);
        void DeletePost(long postID);
        List<DomainModel.Post> GetUserPost(DomainModel.User user);
        List<DomainModel.Comment> GetPostComment(DomainModel.Post post);
        DomainModel.Post FindPost(long postID);
        List<DomainModel.Post> GetPostList();
    }
}
