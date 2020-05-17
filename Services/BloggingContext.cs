using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace BlogsConsole.Models
{
    public class BloggingContext : DbContext
    {
        public BloggingContext() : base("name=BlogContext") { }

        public DbSet<Blog> Blogs { get; set; }
        public DbSet<Post> Posts { get; set; }

        public void AddBlog(Blog blog)
        {
            this.Blogs.Add(blog);
            this.SaveChanges();
        }

        public void UpdateBlog(Blog blog)
        {
            this.Entry(blog);
            this.SaveChanges();
        }


        public void DeleteBlog(Blog blog)
        {
            this.Blogs.Remove(blog);
            var query = this.Posts.Where(p => p.BlogId == blog.BlogId);
            foreach (var post in query)
            {
                this.Posts.Remove(post);
            }
            this.SaveChanges();
        }

        public IOrderedEnumerable<Blog> GetBlogs(string order)
        {
            if (order == "name")
            {
                return this.Blogs.ToList().OrderBy(b => b.Name);
            }
            else
            {
                return this.Blogs.ToList().OrderBy(b => b.BlogId);
            }
            
        }

        public Blog FindBlog(int blogId)
        {
            // return this.Blogs.FirstOrDefault(b => b.BlogId == blogId);

            return this.Blogs.Find(blogId);
        }
        public Blog FindBlog(string blogName)
        {
            return this.Blogs.FirstOrDefault(b => b.Name == blogName);
        }

        public bool BlogExist(string blogName)
        {
            // this is failing!
            if (this.Blogs.Select(b => b.Name == blogName) != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool BlogExist(int id)
        {
            if (this.Blogs.Select(b => b.BlogId == id) != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public void AddPost(Post post)
        {
            this.Posts.Add(post);
            this.SaveChanges();
        }

        public void UpdatePost(Post post)
        {
            this.Entry(post);
            this.SaveChanges();
        }

        public IOrderedEnumerable<Post> GetPosts(int entry)
        {
            if (entry != 0)
            {
                // returns post(s) to a specific blog
                return this.Posts.Where(b => b.BlogId == entry).ToList().OrderBy(p => p.PostId);
            }
            else
            {
                // returns post(s) to all blogs
                return this.Posts.ToList().OrderBy(p => p.Blog.Name).ThenBy(p => p.PostId);
            }
        }

        public Post FindPost(int postID)
        {
            return this.Posts.Find(postID);
        }

        public void DeletePost(Post post)
        {
            this.Posts.Remove(post);
            this.SaveChanges();
        }

        public bool PostExist(int id)
        {
            // this is failing!
            if (this.Posts.Where(p => p.PostId == id) != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public void SaveDBChanges()
        {
            SaveChanges();
        }
    }
}