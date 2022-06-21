using System;

namespace BlogMvcCore.Storage
{
    public class CommentEntity
    {
        public long ID { get; set; }
        public PostEntity Post { get; set; }
        public long Parent { get; set; }
        public string Author { get; set; }
        public string Text { get; set; }
        public DateTime Date { get; set; }
    }
}
