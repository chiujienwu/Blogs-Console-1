﻿using System.Threading.Tasks;

namespace BlogsConsole.Models
{
    public class Post
    {
        public int PostId { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }

        public int BlogId { get; set; }
        public Blog Blog { get; set; }

        public Post() { }

        public Post(int blogId, Blog blog)
        {
            this.BlogId = blogId;
            this.Blog = blog;
        }
    }
}