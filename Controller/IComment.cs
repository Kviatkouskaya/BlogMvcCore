using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogMvcCore.Storage
{
    public interface IComment
    {
        void AddComment(DomainModel.Comment comment);
        void UpdateComment(long commentID, string text);
        void DeleteComment(long commentID);
    }
}
