using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Configuration;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using SkolniProjekt.Model;
using static System.Console;

namespace SkolniProjekt
{
    class Program
    {
        private static Database db = new Database("books.txt", "users.txt");
        private static Auth auth = new Auth(db);

        static void Main(string[] args)
        {
            WriteLine("Welcome to the library management system...");

            WriteLine("\nPlease log in:");
            LogInSequence();

            WriteLine("\nCommand processor initialized...");
            WriteLine("Type 'help' for help.");

            CommandLoop();
        }

        private static void CommandLoop()
        {
            bool exitting = false;
            string input;

            while (!exitting)
            {
                Write("> ");
                switch ((input = ReadLine())?.ToLower())
                {
                    case "help":
                        ShowHelp();
                        break;
                    case "login":
                        auth.LogOut();
                        LogInSequence();
                        break;
                    case "exit":
                        exitting = true;
                        break;
                    case "user":
                        WriteLine(auth.CurrentUser.Username);
                        break;
                    case "user create":
                        SignUpSequence();
                        break;
                    case "user delete":
                        UserDeleteSequence();
                        break;
                    case "user password":
                        ChangePasswordSequence();
                        break;
                    case "user list":
                        WriteLine("{0,-20} {1,5}\n", "Username", "Admin");
                        foreach (var u in db.Users)
                        {
                            string admin = u.Admin ? "Yes" : "No";
                            WriteLine("{0,-20} {1,5}", u.Username, admin);
                        }
                        break;
                    case "book create":
                        BookCreateSequence();
                        break;
                    case "book delete":
                        BookDeleteSequence();
                        break;
                    case "book edit":
                        BookEditSequence();
                        break;
                    case "book list":
                        WriteLine("{0, -10} {1, -10} {2, -10} {3, -10} {4, -10}\n", "Name", "Author", "Genre", "Year", "Country");
                        foreach (var book in db.Books)
                        {
                            WriteLine("{0, -10} {1, -10} {2, -10} {3, -10} {4, -10}", book.Name, book.Author, book.Genre, book.Year.ToString(), book.Country);
                        }
                        break;
                    case "book query":
                        QuerySequence();
                        break;
                    default:
                        WriteLine($"Unknown command '{input}'. Type 'help' for help.");
                        break;
                }
            }
        }

        #region User management

        private static void LogInSequence()
        {
            bool success = false;

            while (!success)
            {
                Write("Username: ");
                string username = ReadLine();

                Write("Password: ");
                string password = SilentReadLine();

                success = auth.LogIn(username, password);

                if (!success)
                    WriteLine("Wrong username or password. Please try again...");
            }

            WriteLine($"Welcome {auth.CurrentUser.Username}!");
        }

        private static void ChangePasswordSequence()
        {
            string pass1 = "a";
            string pass2 = "b";

            while (pass1 != pass2)
            {
                Write("New password: ");
                pass1 = SilentReadLine();
                Write("New password confirmation: ");
                pass2 = SilentReadLine();

                if (pass1 != pass2)
                    WriteLine("Password mismatch. Please try again...");
            }

            auth.CurrentUser.ChangePassword(pass1);
            db.SaveUsersDb();

            WriteLine("Password changed.");
        }

        private static void UserDeleteSequence()
        {
            if (!auth.IsAdmin)
            {
                WriteLine("This can be done only by admins.");
                return;
            }

            Write("Username to delete: ");
            string username = ReadLine();

            if (username == auth.CurrentUser.Username)
            {
                WriteLine("Cannot delete self!");
                return;
            }

            var userList = (from User u in db.Users
                where u.Username == username
                select u).ToList();



            if (userList.Count == 0)
            {
                WriteLine("No such user.");
                return;
            }

            db.Users.Remove(userList.First());
            db.SaveUsersDb();
            WriteLine("User deleted.");
        }

        private static void SignUpSequence()
        {
            if (!auth.IsAdmin)
            {
                WriteLine("This can be done only by admins.");
                return;
            }

            bool success = false;
            while (!success)
            {
                Write("Username: ");
                string username = ReadLine();

                string pass1 = "a";
                string pass2 = "b";

                while (pass1 != pass2)
                {
                    Write("Password: ");
                    pass1 = SilentReadLine();
                    Write("Password confirmation: ");
                    pass2 = SilentReadLine();

                    if (pass1 != pass2)
                        WriteLine("Password mismatch. Please try again...");
                }

                bool admin = false;
                Write("Is the new user admin (y/n)? ");
                string adminString = ReadLine()?.ToLower();
                if (adminString == "y")
                    admin = true;

                success = auth.SignUp(username, pass1, admin);

                if (!success)
                    WriteLine($"User named {username} already exists. Please try again...");
            }

            WriteLine("User successfully created.");
        }

        #endregion

        #region Database management

        private static void BookCreateSequence()
        {
            if (!auth.IsAdmin)
            {
                WriteLine("This can be done only by admins.");
                return;
            }

            Write("Name: ");
            string name = ReadLine();
            Write("Author: ");
            string author = ReadLine();
            Write("Genre: ");
            string genre = ReadLine();

            bool parsed = false;
            int year = 0;

            while (!parsed)
            {
                Write("Year: ");
                string yearString = ReadLine();
                parsed = Int32.TryParse(yearString, out year);

                if (!parsed || year > 3000 || year < 0)
                    WriteLine("Please enter valid number between 0 and 3000");
            }

            Write("Country: ");
            string country = ReadLine();

            var bookQueryList = (from Book b in db.Books
                where b.Name == name
                select b).ToList();

            if (bookQueryList.Count > 0)
            {
                WriteLine("This book record is already present in the database.");
                return;
            }

            Book newBook = new Book(name, author, genre, year, country);
            db.Books.Add(newBook);
            db.SaveBooksDb();
            WriteLine("Book record successfully created.");
        }

        private static void BookDeleteSequence()
        {
            if (!auth.IsAdmin)
            {
                WriteLine("This can be done only by admins.");
                return;
            }

            Write("Bookname to delete: ");
            string name = ReadLine();

            var bookList = (from Book b in db.Books
                where b.Name == name
                select b).ToList();

            if (bookList.Count == 0)
            {
                WriteLine("No such book record in the database.");
                return;
            }

            db.Books.Remove(bookList.First());
            db.SaveBooksDb();
            WriteLine("Book deleted.");
        }

        private static void BookEditSequence()
        {
            if (!auth.IsAdmin)
            {
                WriteLine("This can be done only by admins.");
                return;
            }

            Write("Bookname to edit: ");
            string name = ReadLine();

            var bookList = (from Book b in db.Books
                            where b.Name == name
                            select b).ToList();

            if (bookList.Count == 0)
            {
                WriteLine("No such book record in the database.");
                return;
            }

            Write("New name: ");
            string newName = ReadLine();
            Write("New author: ");
            string author = ReadLine();
            Write("New genre: ");
            string genre = ReadLine();

            bool parsed = false;
            int year = 0;

            while (!parsed)
            {
                Write("New year: ");
                string yearString = ReadLine();
                parsed = Int32.TryParse(yearString, out year);

                if (!parsed || year > 3000 || year < 0)
                    WriteLine("Please enter valid number between 0 and 3000");
            }

            Write("New country: ");
            string country = ReadLine();

            var bookQueryList = (from Book b in db.Books
                                 where b.Name == newName
                                 select b).ToList();

            if (bookQueryList.Count > 0)
            {
                WriteLine("A book record with this name is already present in the database.");
                return;
            }

            Book editee = bookList.First();
            editee.Name = newName;
            editee.Author = author;
            editee.Country = country;
            editee.Genre = genre;
            editee.Year = year;

            db.Books.Remove(bookList.First());
            db.Books.Add(editee);
            db.SaveBooksDb();
            WriteLine("Edit saved.");
        }

        private static void QuerySequence()
        {
            WriteLine("Welcome to the query builder!");
            WriteLine("Leave the fields blank in order to ignore them in the query.");

            Write("Name: ");
            string name = ReadLine();
            Write("Author: ");
            string author = ReadLine();
            Write("Genre: ");
            string genre = ReadLine();

            bool parsed = false;
            string yearString = "a";
            int yearBeg = 0;

            while (!parsed && yearString != "")
            {
                Write("Year from: ");
                yearString = ReadLine();
                parsed = Int32.TryParse(yearString, out yearBeg);

                if ((!parsed && yearString != "") || yearBeg > 3000 || yearBeg < 0)
                    WriteLine("Please enter valid number between 0 and 3000");
            }

            parsed = false;
            yearString = "a";
            int yearEnd = 0;

            while (!parsed && yearString != "")
            {
                Write("Year to: ");
                yearString = ReadLine();
                parsed = Int32.TryParse(yearString, out yearEnd);

                if ((!parsed && yearString != "") || yearEnd > 3000 || yearEnd < 0)
                    WriteLine("Please enter valid number between 0 and 3000");
            }

            Write("Country: ");
            string country = ReadLine();

            bool namePresent = name != "";
            bool authorPresent = author != "";
            bool genrePresent = genre != "";
            bool yearBegPresent = yearBeg > 0;
            bool yearEndPresent = yearEnd > 0;
            bool countryPresent = country != "";

            var set = db.Books.AsEnumerable();
            if (namePresent)
                set = set.Where(book => book.Name == name);
            if (authorPresent)
                set = set.Where(book => book.Author == author);
            if (genrePresent)
                set = set.Where(book => book.Genre == genre);
            if (yearBegPresent)
                set = set.Where(book => book.Year >= yearBeg);
            if (yearEndPresent)
                set = set.Where(book => book.Year <= yearEnd);
            if (countryPresent)
                set = set.Where(book => book.Country == country);

            var list = set.ToList();

            WriteLine("{0, -10} {1, -10} {2, -10} {3, -10} {4, -10}\n", "Name", "Author", "Genre", "Year", "Country");
            foreach (var book in list)
            {
                WriteLine("{0, -10} {1, -10} {2, -10} {3, -10} {4, -10}", book.Name, book.Author, book.Genre, book.Year.ToString(), book.Country);
            }
        }

        #endregion

        #region Helpers and misc

        private static void ShowHelp()
        {
            WriteLine("List of commands:");
            WriteLine("\thelp - show this help");
            WriteLine("\tlogin - login as someone else");
            WriteLine("\texit - close the program");
            WriteLine("\tUser management:");
            WriteLine("\t\tuser - show your username");
            WriteLine("\t\tuser create - create a new user");
            WriteLine("\t\tuser delete - delete a user");
            WriteLine("\t\tuser password - change your password");
            WriteLine("\t\tuser list - show list of users");
            WriteLine("\tDatabase management:");
            WriteLine("\t\tbook create - create a new book record");
            WriteLine("\t\tbook delete - delete a book record");
            WriteLine("\t\tbook edit - edit an existing book record");
            WriteLine("\t\tbook list - show list of book records");
            WriteLine("\t\tbook query - query the database");
        }

        private static string SilentReadLine()
        {
            string password = null;
            while (true)
            {
                var key = ReadKey(true);
                if (key.Key == ConsoleKey.Enter)
                    break;
                password += key.KeyChar;
            }
            WriteLine();
            return password;
        }

        #endregion
    }
}
