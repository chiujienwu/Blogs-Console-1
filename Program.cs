using NLog;
using BlogsConsole.Models;
using System;
using System.ComponentModel.Design;
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
            char choice;

            do
            {
                Menu newMenu = new Menu();
                logger.Info("Menu displayed");

                choice = newMenu.GetUserInput();
                logger.Info("User selected menu choice " + choice);

                switch (choice)
                {
                    case '1':

                        logger.Info("Menu choic 1 selected");

                        try
                        {
                            // Display all Blogs from the database
                            //var query = db.Blogs.OrderBy(b => b.Name);
                            var blogs = db.Blogs.ToList().OrderBy(b => b.Name);
                            Console.WriteLine("All blogs in the database:");
                            foreach (var item in blogs)
                            {
                                Console.WriteLine(item.Name);
                            }

                            logger.Info("Blogs displayed");
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e);
                            throw;
                        }

                        break;

                    case '2':

                        logger.Info("Menu choic 2 selected");

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
                        catch (Exception ex)
                        {
                            logger.Error(ex.Message);
                        }

                        break;

                    case '3':

                        logger.Info("Menu choice 3 selected");

                        try
                        {
                            var blogs = db.Blogs.ToList().OrderBy(b => b.BlogId);
                            Console.WriteLine("All blogs in the database: {0}", blogs.Count());
                            foreach (var item in blogs)
                            {
                                Console.Write(item.BlogId + " - ");
                                Console.WriteLine(item.Name);
                            }

                            Console.WriteLine("Enter blog id to enter a post: ");
                            var entry = Convert.ToInt16(Console.ReadLine());

                            var blog = db.Blogs.FirstOrDefault(b => b.BlogId == entry);
                            var blogID = blog.BlogId;

                            Post newPost = new Post(blogID, blog);
                            logger.Info("newPost created with constructor");

                            Console.WriteLine("Enter title for post: ");
                            newPost.Title = Console.ReadLine();

                            Console.WriteLine("Enter post content: ");
                            newPost.Content = Console.ReadLine();

                            db.Posts.Add(newPost);
                            logger.Info("newPost added");
                            db.SaveChanges();
                            logger.Info("db save changes");

                            logger.Info("Post added to db");

                        }
                        catch (Exception ex)
                        {
                            logger.Error(ex.Message);
                        }

                        break;
                }
            } while (choice != '0');

            logger.Info("Menu choic 0 selected");
            logger.Info("Program ended");
        }
    }
}
