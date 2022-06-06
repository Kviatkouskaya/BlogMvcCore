using BlogMvcCore.DomainModel;
using System.Collections.Generic;

namespace BlogMvcCore.Services
{
    public class UserService
    {
        private readonly IUserAction userActionContext;
        public UserService(IUserAction action) => userActionContext = action;

        public User VisitUserPage(string login)
        {
            User user = userActionContext.FindUser(login);
            user.Posts = userActionContext.ReturnUserPost(user);

            return user;
        }

        public List<User> ReturnUsers() => userActionContext.ReturnUsersList();
    }
}
