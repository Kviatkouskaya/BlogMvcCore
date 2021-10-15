using System;
using System.Collections.Generic;

namespace BlogMvcCore.Storage
{
    public class Post
    {
        public long ID { get; set; }
        public User Author { get; set; }
        public string Title { get; set; }
        public string Text { get; set; }
        public DateTime Date { get; set; }
    }
}
