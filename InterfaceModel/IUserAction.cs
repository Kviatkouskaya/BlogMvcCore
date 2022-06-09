using System;
using System.Collections.Generic;

namespace BlogMvcCore.DomainModel
{
    public interface IUserAction : IDisposable
    {
        public List<User> ReturnUsersList();
        bool LoginUser(string login, string password);
        void Register(User user);
        User FindUser(string login);
        public void AddPost(Post post);
        public void DeletePost(long postID);
        public List<Post> ReturnUserPost(User user);
        public void AddComment(Comment comment);
        public List<Comment> ReturnPostComment(Post post);
        Post FindPost(long postID);
        public List<Post> ReturnPostList();
    }
}
