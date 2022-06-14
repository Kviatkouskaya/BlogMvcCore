using BlogMvcCore.DomainModel;
using System.Collections.Generic;

namespace BlogMvcCore.Services
{
    public class UserService
    {
        private readonly IUserAction userActionContext;
        private readonly IPostAction postAction;
        public UserService(IUserAction action, IPostAction postAction)
        {
            userActionContext = action;
            this.postAction = postAction;
        }

        public virtual User VisitUserPage(string login)
        {
            User user = userActionContext.FindUser(login);
            user.Posts = postAction.GetUserPost(user);

            return user;
        }

        public virtual List<User> ReturnUsers() => userActionContext.GetUsersList();
    }
}
