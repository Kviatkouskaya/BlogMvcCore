using System;
using System.Collections.Generic;
using BlogMvcCore.DomainModel;

namespace BlogMvcCore.Storage
{
    public interface IPostRepository : IDisposable
    {
        void AddPost(DomainModel.PostDomainModel post);
        void DeletePost(long postID);
        List<DomainModel.PostDomainModel> GetUserPost(DomainModel.UserDomainModel user);
        List<DomainModel.CommentDomainModel> GetPostComment(DomainModel.PostDomainModel post);
        DomainModel.PostDomainModel FindPost(long postID);
        List<DomainModel.PostDomainModel> GetPostList();
    }
}
