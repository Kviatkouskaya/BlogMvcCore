using System;
using System.Collections.Generic;

namespace BlogMvcCore.DomainModel
{
    public interface IUserAction : IDisposable
    {
        List<User> GetUsersList();
        bool LoginUser(string login, string password);
        void Register(User user);
        User FindUser(string login);
        void AddPost(Post post);
        void DeletePost(long postID);
        List<Post> GetUserPost(User user);
        void AddComment(Comment comment);
        void UpdateComment(long commentID, string text);
        void DeleteComment(long commentID);
        List<Comment> GetPostComment(Post post);
        Post FindPost(long postID);
        List<Post> GetPostList();
    }
}
