﻿using System.Collections.Generic;
using System;

namespace BlogMvcCore.DomainModel
{
    public class Post
    {
        public long ID { get; set; }
        public User Author { get; set; }
        public string Title { get; set; }
        public string Text { get; set; }
        public DateTime Date { get; set; }
        public List<Comment> Comments { get; set; }
    }
}
