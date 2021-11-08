using System.Collections.Generic;

namespace BlogMvcCore.DomainModel
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
        public string FirstName { get; set; }
        public string SecondName { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
        public List<Post> Posts { get; set; }
        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }
            User user = (User)obj;
            if (Login == user.Login)
            {
                if (FirstName == user.FirstName && SecondName == user.SecondName &&
                    Password == user.Password && Posts == user.Posts)
                {
                    return true;
                }
            }
            return false;
        }

        public override int GetHashCode()
        {
            // TODO: write your implementation of GetHashCode() here
            throw new System.NotImplementedException();
            return base.GetHashCode();
        }
    }
}
