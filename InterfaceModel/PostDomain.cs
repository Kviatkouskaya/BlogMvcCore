using System.Collections.Generic;
using System;

namespace BlogMvcCore.DomainModel
{
    public class PostDomain
    {
        public long ID { get; set; }
        public UserDomain Author { get; set; }
        public string Title { get; set; }
        public string Text { get; set; }
        public DateTime Date { get; set; }
        public List<CommentWithLevel> Comments { get; set; }
    }
}
