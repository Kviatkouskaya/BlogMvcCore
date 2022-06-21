using System.Collections.Generic;

namespace BlogMvcCore.DomainModel
{
    public class UserDomain
    {
        public UserDomain(string firstName, string secondName, string login, string password)
        {
            FirstName = firstName;
            SecondName = secondName;
            Login = login;
            Password = password;
        }
        public string FirstName { get; set; }
        public string SecondName { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
        public List<PostDomain> Posts { get; set; }
    }
}
