using System;
using System.Collections.Generic;

namespace BlogMvcCore.Models
{
    public class RepositoryInMemory : IUserAction
    {
        private static readonly List<User> allowedUsers = new() { new User("Admin", "System", "admin", "12345678") };
        private static readonly List<Post> postList = new();
        private static readonly List<Comment> commentList = new();
        public int LoginUser(string login, string password)
        {
            var count = 0;
            foreach (var item in allowedUsers)
            {
                if (item.Login == login &&
                   item.Password == password)
                {
                    return count += 1;
                }
            }
            return count;
        }
        public void Register(User user)
        {
            if (user.FirstName != string.Empty && user.SecondName != string.Empty)
            {
                if (user.Login != string.Empty && user.Password != string.Empty)
                {
                    allowedUsers.Add(user);
                }
            }
        }
        public User FindUser(string login)
        {
            foreach (var item in allowedUsers)
            {
                if (item.Login == login)
                {
                    return item;
                }
            }
            return null;
        }

        public void AddPost(Post post)
        {
            postList.Add(post);
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public List<Post> ReturnUserPost(User user)
        {
            List<Post> userPost = new();
            foreach (var item in postList)
            {
                if (item.Author == user)
                {
                    userPost.Add(item);
                }
            }
            return userPost;
        }

        public void AddComment(Comment comment)
        {
            commentList.Add(comment);
        }

        public List<Comment> ReturnPostComment(Post post)
        {
            List<Comment> postComment = new();
            foreach (var item in commentList)
            {
                if (item.Post == post)
                {
                    postComment.Add(item);
                }
            }
            return postComment;
        }

        public Post FindPost(long postID)
        {
            throw new NotImplementedException();
        }

        public int CheckLoginDuplicate(string login)
        {
            throw new NotImplementedException();
        }

        public List<User> ReturnUsersList()
        {
            throw new NotImplementedException();
        }
    }
}
