using System;
using System.Collections.Generic;
using BlogMvcCore.DomainModel;
namespace BlogMvcCore.Models
{
    public class RepositoryInMemory : IUserAction
    {
        public readonly List<User> allowedUsers;
        public RepositoryInMemory()
        {
            allowedUsers = new()
            {
                new User("Admin", "System", "admin", "12345678") { Posts = new() },
                new User("Adam", "Smith", "adamsm", "qweasdzxc") { Posts = new() },
                new User("Frank", "Right", "right", "123qwe123") { Posts = new() },
                new User("First", "Second", "first", "12312312") { Posts = new() }
            };
        }

        public bool LoginUser(string login, string password)
        {
            User user = allowedUsers.Find(u => u.Login == login && u.Password == password);
            return user == null;
        }

        public void Register(User user)
        {
            allowedUsers.Add(user);
        }

        public User FindUser(string login)
        {
            User user = allowedUsers.Find(u => u.Login == login);
            return user ?? null;
        }

        public void AddPost(Post post)
        {
            if (post.Author == null) return;
            foreach (var item in allowedUsers)
            {
                if (item.Login == post.Author.Login)
                {
                    if (item.Posts == null)
                    {
                        item.Posts = new List<Post>();
                    }
                    item.Posts.Add(post);
                    return;
                }
            }
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public List<Post> ReturnUserPost(User user)
        {
            return user.Posts;
        }

        public void AddComment(Comment comment)
        {
            foreach (var user in allowedUsers)
            {
                foreach (var item in user.Posts)
                {
                    if (item == comment.Post)
                    {
                        item.Comments.Add(comment);
                        return;
                    }
                }
            }
        }

        public List<Comment> ReturnPostComment(Post post)
        {
            User user = allowedUsers.Find(u => u.Login == post.Author.Login);
            Post userPost = user.Posts.Find(p => p.Title == post.Title);
            return userPost.Comments;
        }

        public Post FindPost(long postID)
        {
            Post post = new();
            foreach (var item in allowedUsers)
            {
                if (item.Posts != null)
                {
                    post = item.Posts.Find(p => p.ID == postID);
                }
            }
            return null;
        }

        public int CheckLoginDuplicate(string login)
        {
            var result = allowedUsers.Find(u => u.Login == login);
            return result is default(User) ? 0 : 1;
        }

        public List<User> ReturnUsersList()
        {
            return allowedUsers;
        }
    }
}
