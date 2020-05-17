using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogsConsole.Models

{
    public class Menu
    {
        public Menu()
        {
            Console.WriteLine("------- MENU --------");
            Console.WriteLine("(1) Display all blogs");
            Console.WriteLine("(2) Add a blog");
            Console.WriteLine("(3) Create a post to a blog");
            Console.WriteLine("(4) Display posts for a blog");
            Console.WriteLine("(5) Edit Blog");
            Console.WriteLine("(6) Edit Post");
            Console.WriteLine("(7) Delete Blog");
            Console.WriteLine("(8) Delete Post");
            Console.WriteLine("(0) Exit program");
        }

        public char GetUserInput()
        {
            char selection;

            Console.Write("?");
            while (!IsValidInput(Console.ReadKey(true).KeyChar, out selection))
            {
                Console.WriteLine($"Invalid input: {selection}");
                Console.WriteLine();
                Console.WriteLine("Please enter a valid option.");
                Console.Write("?");
            }

            Console.WriteLine("You have selected {0}", selection);
            return selection;
        }

        private bool IsValidInput(char input, out char selection)
        {
            char[] validValues = { '1', '2', '3', '4', '5', '6', '7', '8', '0' };

            selection = Char.ToUpper(input);
            if (validValues.Contains(input))
            {
                return true;
            }

            return false;
        }
    }
}