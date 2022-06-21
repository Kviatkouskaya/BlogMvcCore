using System;
using System.Collections.Generic;
using BlogMvcCore.DomainModel;

namespace BlogMvcCore.Storage
{
    public interface IPostRepository : IDisposable
    {
        void AddPost(DomainModel.PostDomain post);
        void DeletePost(long postID);
        List<DomainModel.PostDomain> GetUserPost(DomainModel.UserDomain user);
        List<DomainModel.CommentDomain> GetPostComment(DomainModel.PostDomain post);
        DomainModel.PostDomain FindPost(long postID);
        List<DomainModel.PostDomain> GetPostList();
    }
}
