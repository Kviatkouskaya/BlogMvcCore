using BlogMvcCore.DomainModel;
using System.Collections.Generic;

namespace BlogMvcCore.Services
{
    public class UserService
    {
        private readonly IUserAction UserActionContext;
        public UserService(IUserAction action) => UserActionContext = action;

        public virtual User VisitUserPage(string login)
        {
            User user = UserActionContext.FindUser(login);
            user.Posts = UserActionContext.ReturnUserPost(user);

            return user;
        }

        public virtual List<User> ReturnUsers() => UserActionContext.ReturnUsersList();
    }
}
