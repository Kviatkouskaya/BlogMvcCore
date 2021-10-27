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
                new User("Admin", "System", "admin", "12345678"),
                new User("Adam", "Smith", "adamsm", "qweasdzxc"),
                new User("Frank", "Right", "right", "123qwe123")
            };
        }

        public int LoginUser(string login, string password)
        {
            User user = allowedUsers.Find(u => u.Login == login && u.Password == password);
            return user == null ? 0 : 1;
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
            foreach (var item in allowedUsers)
            {
                if (item.Login == user.Login)
                {
                    return item.Posts;
                }
            }
            return null;
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
            throw new NotImplementedException();
        }

        public int CheckLoginDuplicate(string login)
        {
            var result = allowedUsers.Find(u => u.Login == login);
            return result == null ? 0 : 1;
        }

        public List<User> ReturnUsersList()
        {
            throw new NotImplementedException();
        }
    }
}
