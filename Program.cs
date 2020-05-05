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

                            logger.Info("Menu choic 1 selected");

                            try
                            {
                                // Display all Blogs from the database
                                //var query = db.Blogs.OrderBy(b => b.Name);
                                var blogs = db.Blogs.ToList().OrderBy(b => b.Name);

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

                                var blog = new Blog { Name = name };

                                db.AddBlog(blog);
                                db.SaveChanges();
                                logger.Info("Blog added - {name}", name);
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
                                var blogs = db.Blogs.ToList().OrderBy(b => b.BlogId);
                                
                                DisplayBlogs(blogs);

                                try
                                {
                                    Console.WriteLine("Enter blog id to enter a post: ");
                                    var entry = Convert.ToInt16(Console.ReadLine());
                                    var blog = db.Blogs.FirstOrDefault(b => b.BlogId == entry);
                                    var blogID = blog.BlogId;

                                    try
                                    {
                                        Post newPost = new Post(blogID, blog);
                                        logger.Info("newPost created with constructor");

                                        Console.WriteLine("Enter title for post: ");
                                        newPost.Title = Console.ReadLine();

                                        Console.WriteLine("Enter post content: ");
                                        newPost.Content = Console.ReadLine();

                                        try
                                        {
                                            db.Posts.Add(newPost);
                                            logger.Info("newPost added");
                                            db.SaveChanges();
                                            logger.Info("db save changes");

                                            logger.Info("Post added to db");
                                        }
                                        catch (Exception ae)
                                        {
                                            Console.WriteLine(ae + "Cannot add new post.");
                                            logger.Error(ae.Message);
                                        }

                                    }
                                    catch (Exception ae)
                                    {
                                        Console.WriteLine("Cannot create new post.  Check Blog ID.");
                                        logger.Error(ae.Message);
                                    }

                                }
                                catch (Exception ae)
                                {
                                    Console.WriteLine("Blog ID not found");
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
                                var blogs = db.Blogs.ToList().OrderBy(b => b.BlogId);
                                
                                Console.WriteLine("Select the blog's posts to display:");
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

                                if (entry == 0)
                                {
                                    var posts = db.Posts.ToList().OrderBy(p => p.Blog.Name).ThenBy(p =>p.PostId);
                                    Console.WriteLine("Sorted by Blog Name and then by PostID");
                                    DisplayPosts(posts);
                                }
                                else
                                {
                                    var posts = db.Posts.Where(b => b.BlogId == entry).ToList().OrderBy(p => p.PostId);

                                    DisplayPosts(posts);
                                }

                            }
                            catch (Exception ae)
                            {
                                Console.WriteLine("Cannot retrieve posts from db.");
                                logger.Error(ae.Message);
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
                Console.WriteLine("Title: " + item.Title);
                Console.WriteLine("Content: " + item.Content);
            }
            Console.WriteLine("{0} post(s) returned", postsToDisplay.Count());

            logger.Info("Posts displayed");
        }

        public static void DisplayBlogs(IOrderedEnumerable<Blog> blogsToDisplay)
        {
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

