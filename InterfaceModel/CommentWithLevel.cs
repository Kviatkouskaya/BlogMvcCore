using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogMvcCore.DomainModel
{
    public class CommentWithLevel
    {
        public CommentDomain Comment { get; set; }
        public int Level { get; set; }
    }
}
