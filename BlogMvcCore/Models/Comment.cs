using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlogMvcCore.Models
{
    public class Comment
    {
        public long ID { get; set; }
        public Post PostID { get; set; }
        public string Author { get; set; }
        public string Text { get; set; }
        public DateTime Date { get; set; }
    }
}
