using System;
using System.Collections.Generic;

namespace BlogMvcCore.DomainModel
{
    public interface IUserAction : IDisposable
    {
        List<User> GetUsersList();
        User FindUser(string login);
        
        void AddComment(Comment comment);
        void UpdateComment(long commentID, string text);
        void DeleteComment(long commentID);
        
    }
}
