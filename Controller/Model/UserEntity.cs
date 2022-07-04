namespace BlogMvcCore.Storage
{
    public class UserEntity
    {
        public UserEntity(string firstName, string secondName, string login, string password,string salt)
        {
            FirstName = firstName;
            SecondName = secondName;
            Login = login;
            Password = password;
            Salt = salt;
        }
        public long ID { get; set; }
        public string FirstName { get; set; }
        public string SecondName { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
        public string Salt { get; set; }
    }
}
