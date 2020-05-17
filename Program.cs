using NLog;
using BlogsConsole.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Data.Entity.Validation;
using System.Linq;
using System.Xml.Serialization;

namespace BlogsConsole
{
    class MainClass
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
        public static void Main(string[] args)
        {
            logger.Info("Program started");
            var db = new BloggingContext();
            db.Database.CreateIfNotExists();
            logger.Info("Blogging context instantiated");
            char choice;

            do
            {
                Menu newMenu = new Menu();
                logger.Info("Menu displayed");

                choice = newMenu.GetUserInput();
                logger.Info("User selected menu choice " + choice);

                try
                {

                    switch (choice)
                    {
                        case '1':

                            logger.Info("Menu choice 1 selected");

                            try
                            {
                                var blogs = db.GetBlogs("name");
                                Console.WriteLine("Sorted by Blog Name");
                                DisplayBlogs(blogs);
                            }
                            catch (Exception ae)
                            {
                                Console.WriteLine("Error retrieving blogs from db.");
                                logger.Error(ae.Message);
                            }

                            break;

                        case '2':

                            logger.Info("Menu choice 2 selected");

                            try
                            {
                                // Create and save a new Blog
                                Console.Write("Enter a name for a new Blog: ");
                                var name = Console.ReadLine();

                                //check to see if blog name already exists

                                if (!db.BlogExist(name))
                                {
                                    db.AddBlog(new Blog { Name = name });
                                    logger.Info("Blog added - {name} to db.", name);
                                }
                                else
                                {
                                    Console.WriteLine("Blog name already exists");
                                    logger.Info("Blog name already exists in db.");
                                }
                            }
                            catch (DbEntityValidationException ae)
                            {
                                foreach (var error in ae.EntityValidationErrors)
                                {
                                    Console.WriteLine(
                                        "Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                                        error.Entry.Entity.GetType().Name, error.Entry.State);
                                    foreach (var ve in error.ValidationErrors)
                                    {
                                        Console.WriteLine("- Entity Property: \"{0}\", Error: \"{1}\"",
                                            ve.PropertyName, ve.ErrorMessage);
                                    }
                                }

                                Console.WriteLine("Error creating or adding blog to db.");
                                logger.Error(ae.Message);
                            }

                            break;

                        case '3':

                            logger.Info("Menu choice 3 selected");

                            try
                            {
                                var blogs = db.GetBlogs("name");
                                Console.WriteLine("Sorted by Blog Name");
                                DisplayBlogs(blogs);

                                try
                                {
                                    Console.WriteLine("Enter blog id to enter a post: ");
                                    var entry = Convert.ToInt16(Console.ReadLine());



                                    if (db.BlogExist(entry))
                                    {

                                        var blog = db.FindBlog(entry);
                                        var newPost = new Post(entry, blog);

                                        Console.WriteLine("Enter title for post: ");
                                        newPost.Title = Console.ReadLine();

                                        Console.WriteLine("Enter post content: ");
                                        newPost.Content = Console.ReadLine();

                                        db.AddPost(newPost);

                                        logger.Info("Post added to selected blog.");
                                    }
                                    else
                                    {
                                        logger.Info("Unable to find blog with that ID.");
                                    }

                                }
                                catch (Exception ae)
                                {
                                    logger.Error(ae.Message);
                                }

                            }
                            catch (Exception ae)
                            {
                                Console.WriteLine("Blog list cannot be retrieved from db.");
                                logger.Error(ae.Message);
                            }

                            break;

                        case '4':

                            logger.Info("Menu choice 4 selected");

                            try
                            {
                                var blogs = db.GetBlogs("blogId");
                                Console.WriteLine("Sorted by Blog ID");
                                Console.WriteLine("Select the blog posts to display:");
                                Console.WriteLine("0 - Posts from all blogs");

                                DisplayBlogs(blogs);
                            }
                            catch (Exception ae)
                            {
                                Console.WriteLine("Cannot retrieve blogs from db.");
                                logger.Error(ae.Message);
                            }

                            try
                            {
                                Console.WriteLine("Enter blog id to display a post: ");
                                var entry = Convert.ToInt16(Console.ReadLine());
                                logger.Info("Selected Blog ID  = " + entry);

                                var posts = db.GetPosts(entry);
                                DisplayPosts(posts);

                            }
                            catch (Exception ae)
                            {
                                Console.WriteLine("Cannot retrieve posts from db.");
                                logger.Error(ae.Message);
                            }

                            break;

                        case '5':

                            logger.Info("Menu choice 5 selected");

                            try
                            {
                                var blogs = db.GetBlogs("blogId");
                                Console.WriteLine("Sorted by Blog ID");
                                Console.WriteLine("Select the blog posts to display:");
                                Console.WriteLine("0 - Posts from all blogs");

                                DisplayBlogs(blogs);
                            }
                            catch (Exception ae)
                            {
                                Console.WriteLine("Cannot retrieve blogs from db.");
                                logger.Error(ae.Message);
                            }

                            try
                            {
                                Console.WriteLine("Enter blog id to edit: ");
                                var entry = Convert.ToInt16(Console.ReadLine());
                                logger.Info("Selected Blog ID  = " + entry);


                                if (db.BlogExist(entry))
                                {
                                    var blog = db.FindBlog(entry);
                                    Console.WriteLine("Blog Name : {0}", blog.Name);

                                    Console.Write("Enter new Blog name: ");
                                    var name = Console.ReadLine();

                                    blog.Name = name;

                                    db.UpdateBlog(blog);
                                }
                            }
                            catch (Exception ae)
                            {
                                Console.WriteLine("Cannot retrieve posts from db.");
                                logger.Error(ae.Message);
                            }

                            break;

                        case '6':  // need to review after change to FindPost method

                            logger.Info("Menu choice 6 selected");

                            try
                            {
                                var blogs = db.GetBlogs("name");
                                Console.WriteLine("Sorted by Blog Name");
                                DisplayBlogs(blogs);

                                try
                                {
                                    Console.WriteLine("Enter blog id to update a post: ");
                                    var entry = Convert.ToInt16(Console.ReadLine());

                                    if (db.BlogExist(entry))
                                    {
                                        var blog = db.FindBlog(entry);

                                        //check if there are any posts
                                        if (db.GetPosts(blog.BlogId).Any())
                                        {
                                            DisplayPosts(db.GetPosts(blog.BlogId));
                                            Console.WriteLine("Please enter ID of Post to edit:");
                                            var postID = Convert.ToInt16(Console.ReadLine());

                                            var post = db.FindPost(postID);

                                            if (post != null && post.BlogId == entry)
                                            {
                                                Console.WriteLine("Post Title: {0}", post.Title);
                                                Console.WriteLine("Enter new Title: ");
                                                var title = Console.ReadLine();

                                                Console.WriteLine("Post Content: {0}", post.Content);
                                                Console.WriteLine("Enter new Content");
                                                var content = Console.ReadLine();

                                                post.Title = title;
                                                post.Content = content;

                                                db.UpdatePost(post);
                                            }
                                            else
                                            {
                                                logger.Info("No post found.");
                                            }



                                        }
                                        else
                                        {
                                            Console.WriteLine("There are no posts for this Blog.");
                                        }

                                    }
                                    else
                                    {
                                        logger.Info("Unable to find blog with that ID.");
                                    }

                                }
                                catch (Exception ae)
                                {
                                    logger.Error(ae.Message);
                                }

                            }
                            catch (Exception ae)
                            {
                                Console.WriteLine("Blog list cannot be retrieved from db.");
                                logger.Error(ae.Message);
                            }

                            break;

                        case '7':

                            logger.Info("Menu choice 7 selected");

                            try
                            {
                                var blogs = db.GetBlogs("blogID");
                                Console.WriteLine("Sorted by Blog BlogID");
                                DisplayBlogs(blogs);

                                Console.WriteLine("Enter Blog ID and all the related posts you wish to delete:");
                                var entry = Convert.ToInt16(Console.ReadLine());

                                if (db.BlogExist(entry))
                                {
                                    Console.WriteLine("Are you sure you wish to delete Y/N and enter?");
                                    var confirm = Console.ReadLine().ToUpper();

                                    if (confirm == "Y" || confirm == "YES")
                                    {
                                        db.DeleteBlog(db.FindBlog(entry));
                                        logger.Info("Deletion completed.");
                                    }
                                    else
                                    {
                                        logger.Info("Deletion aborted.");
                                    }
                                }
                                else
                                {
                                    logger.Info("Invalid Blog ID.");
                                }

                            }
                            catch (Exception ae)
                            {
                                Console.WriteLine(ae);
                                throw;
                            }

                            break;

                        case '8':

                            logger.Info("Menu choice 8 selected");

                            try
                            {
                                var blogs = db.GetBlogs("blogID");
                                Console.WriteLine("Sorted by Blog BlogID");
                                DisplayBlogs(blogs);

                                Console.WriteLine("Enter Blog ID for related posts you wish to delete:");
                                var entry = Convert.ToInt16(Console.ReadLine());

                                logger.Info("Blog ID selected : " + entry);

                                if (db.FindBlog(entry) != null)  // convert to FindBlog from BlogExist
                                {
                                    logger.Info("db.BlogExist(entry) value : " + db.BlogExist(entry));
                                    var posts = db.GetPosts(entry);
                                    DisplayPosts(posts);

                                    if (posts.Count() != 0)
                                    {
                                        logger.Info("posts.Count() : " + posts.Count());

                                        Console.WriteLine("Enter Post ID to delete: ");
                                        var postID = Convert.ToInt16(Console.ReadLine());
                                        logger.Info("Post ID selected : " + postID);

                                        if (db.FindPost(postID) != null)  // convert to FindPost from PostExist
                                        {
                                            logger.Info("db.PostExist(postID) : " + db.FindPost(postID));

                                            var post = db.FindPost(postID);

                                            if (post.BlogId == entry)
                                            {
                                                logger.Info("post.BlogId == entry : " + post.BlogId);

                                                Console.WriteLine("Are you sure you wish to delete post? :");
                                                var confirm = Console.ReadLine().ToUpper();

                                                if (confirm == "Y" || confirm == "YES")
                                                {
                                                    db.DeletePost(post);
                                                    logger.Info("Deletion completed.");
                                                }
                                                else
                                                {
                                                    logger.Info("Deletion aborted.");
                                                }

                                            }

                                            else
                                            {
                                                logger.Info("Post ID does not belong to Blog selected.");
                                            }
                                        }
                                        else
                                        {
                                            logger.Info("Post ID does not exist.");
                                        }

                                    }
                                    else
                                    {
                                        logger.Info("There are no posts to delete.");
                                    }
                                }
                                else
                                {
                                    logger.Info("Invalid Blog ID.");
                                }
                            }
                            catch (Exception ae)
                            {
                                Console.WriteLine(ae);
                            }

                            break;
                    }
                }

                catch (AggregateException ae)
                {
                    foreach (var e in ae.InnerExceptions)
                    {
                        Console.WriteLine("{0}:\n   {1}", e.GetType().Name, e.Message);
                    }
                }

            } while (choice != '0');

            logger.Info("Menu choic 0 selected");
            logger.Info("Program ended");
        }

        // method to output list by taking in a list, any list

        public static void DisplayPosts(IOrderedEnumerable<Post> postsToDisplay)
        {

            foreach (var item in postsToDisplay)
            {
                Console.WriteLine("Blog: " + item.Blog.Name);
                Console.WriteLine("PostID: " + item.PostId);
                Console.WriteLine("Title: " + item.Title);
                Console.WriteLine("Content: " + item.Content);
            }
            Console.WriteLine("{0} post(s) returned", postsToDisplay.Count());

            logger.Info("Posts displayed");
        }

        public static void DisplayBlogs(IOrderedEnumerable<Blog> blogsToDisplay)
        {
            Console.WriteLine("Blog ID - Blog Name");
            foreach (var item in blogsToDisplay)
            {
                Console.Write(item.BlogId + " - ");
                Console.WriteLine(item.Name);
            }
            Console.WriteLine("{0} blogs(s) returned", blogsToDisplay.Count());

            logger.Info("Blogs displayed");
        }

    }
}

