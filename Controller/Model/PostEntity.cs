using System;

namespace BlogMvcCore.Storage
{
    public class PostEntity
    {
        public long ID { get; set; }
        public UserEntity Author { get; set; }
        public string Title { get; set; }
        public string Text { get; set; }
        public DateTime Date { get; set; }
    }
}
