using BlogMvcCore.DomainModel;
using System.Collections.Generic;

namespace BlogMvcCore.Services
{
    public class UserService
    {
        private readonly Storage.IUserRepository userRepository;
        private readonly Storage.IPostRepository postRepository;
        public UserService(Storage.IUserRepository userRepository, Storage.IPostRepository postRepository)
        {
            this.userRepository = userRepository;
            this.postRepository = postRepository;
        }

        public virtual UserDomain VisitUserPage(string login)
        {
            UserDomain user = userRepository.FindUser(login);
            user.Posts = postRepository.GetUserPost(user);

            return user;
        }

        public virtual List<UserDomain> ReturnUsers() => userRepository.GetUsersList();
    }
}
