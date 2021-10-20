using System.Collections.Generic;

namespace BlogMvcCore.Storage
{
    public class User
    {
        public User(string firstName, string secondName, string login, string password)
        {
            FirstName = firstName;
            SecondName = secondName;
            Login = login;
            Password = password;
        }
        public long ID { get; set; }
        public string FirstName { get; set; }
        public string SecondName { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
    }
}
