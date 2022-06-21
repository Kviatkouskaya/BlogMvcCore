using System;

namespace BlogMvcCore.DomainModel
{
    public class CommentDomain
    {
        public long ID { get; set; }
        public PostDomain Post { get; set; }
        public long Parent { get; set; }
        public string Author { get; set; }
        public string Text { get; set; }
        public DateTime Date { get; set; }
    }
}
