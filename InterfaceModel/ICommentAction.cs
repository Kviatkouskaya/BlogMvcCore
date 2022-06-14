using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogMvcCore.DomainModel
{
    public interface ICommentAction
    {
        void AddComment(Comment comment);
        void UpdateComment(long commentID, string text);
        void DeleteComment(long commentID);
    }
}
