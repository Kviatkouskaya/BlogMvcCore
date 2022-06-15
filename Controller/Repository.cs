using BlogMvcCore.DomainModel;
using System.Collections.Generic;
using System.Linq;

namespace BlogMvcCore.Storage
{
    public class Repository : IAuthenticationAction, IUserAction, ICommentAction
    {
        private readonly DbContext context;
        public Repository(DbContext context) => this.context = context;

        public DomainModel.User FindUser(string login)
        {
            var user = context.BlogUsers.Where(u => u.Login == login).First();

            return new(user.FirstName, user.SecondName, user.Login, user.Password);
        }



        public bool LoginUser(string login, string password)
        {
            var result = context.BlogUsers.Where(u => u.Login == login && u.Password == password)
                                          .Count();
            return result != 0;
        }

        public void Register(DomainModel.User newUser)
        {
            var user = new User(newUser.FirstName, newUser.SecondName, newUser.Login, newUser.Password);
            context.BlogUsers.Add(user);
            context.SaveChanges();
        }

        public void Dispose() => context.Dispose();



        public void AddComment(DomainModel.Comment comment)
        {
            Post entityPost = context.Posts.Find(comment.Post.ID);
            context.Attach(entityPost);
            context.Comments.Add(new Comment
            {
                ID = comment.ID,
                Parent = comment.Parent,
                Post = entityPost,
                Author = comment.Author,
                Text = comment.Text,
                Date = comment.Date
            });
            context.SaveChanges();
        }

        public void UpdateComment(long commentID, string commentText)
        {
            var entityComment = context.Comments.Find(commentID);
            entityComment.Text = commentText;
            context.Comments.Update(entityComment);
            context.SaveChanges();
        }

        public void DeleteComment(long commentID)
        {
            var entity = context.Comments.FirstOrDefault(x => x.ID == commentID);
            if (entity != null)
                context.Entry(entity).State = Microsoft.EntityFrameworkCore.EntityState.Deleted;
            context.SaveChanges();
        }


        public List<DomainModel.User> GetUsersList()
        {
            var userDomainList = context.BlogUsers.Select(u => new DomainModel.User(u.FirstName, u.SecondName, u.Login, u.Password))
                                                  .ToList();
            return userDomainList;
        }

    }
}