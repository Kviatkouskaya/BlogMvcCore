﻿using System;

namespace BlogMvcCore.DomainModel
{
    public class Comment
    {
        public long ID { get; set; }
        public Post Post { get; set; }
        public string Author { get; set; }
        public string Text { get; set; }
        public DateTime Date { get; set; }
    }
}
