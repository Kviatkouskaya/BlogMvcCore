using BlogMvcCore.DomainModel;
using System.Collections.Generic;

namespace BlogMvcCore.Services
{
    public class UserService
    {
        private readonly Storage.IUser context;
        private readonly IPostAction postAction;
        public UserService(Storage.IUser action, IPostAction postAction)
        {
            context = action;
            this.postAction = postAction;
        }

        public virtual User VisitUserPage(string login)
        {
            User user = context.FindUser(login);
            user.Posts = postAction.GetUserPost(user);

            return user;
        }

        public virtual List<User> ReturnUsers() => context.GetUsersList();
    }
}
