using System;
using System.Collections.Generic;

namespace BlogMvcCore.Models
{
    public interface IUserAction : IDisposable
    {
        int LoginUser(string login, string password);
        void Register(User user);
        User FindUser(string login);
        public void AddPost(Post post);
        public List<Post> ReturnUserPost(User user);
        public void AddComment(Comment comment);
        public List<Comment> ReturnPostComment(Post post);
        Post FindPost(long postID);
        int CheckLoginDuplicate(string login);
    }
}
