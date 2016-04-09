using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkolniProjekt.Model
{
    class Database
    {
        public List<Book> Books { get; private set; } = new List<Book>();
        public List<User> Users { get; private set; } = new List<User>();

        private readonly string booksFilePath;
        private readonly string usersFilePath;

        public Database(string booksFilePath, string usersFilePath)
        {
            this.booksFilePath = booksFilePath;
            this.usersFilePath = usersFilePath;

            LoadDb();
        }

        private void LoadDb()
        {
            if (!File.Exists(booksFilePath))
                File.Create(booksFilePath).Close();
            if (!File.Exists(usersFilePath))
                File.Create(usersFilePath).Close();

            using (var reader = new StreamReader(booksFilePath))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    var props = line.Split(new[] {';'});
                    Book b = new Book(props[0], props[1], props[2], int.Parse(props[3]), props[4]);
                    Books.Add(b);
                }

                reader.Close();
            }

            using (var reader = new StreamReader(usersFilePath))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    var props = line.Split(new[] {';'});
                    User u = new User(props[0], props[1], bool.Parse(props[2]));
                    Users.Add(u);
                    
                }

                reader.Close();
            }
        }

        public void SaveBooksDb()
        {
            using (var writer = new StreamWriter(booksFilePath))
            {
                foreach (var book in Books)
                {
                    writer.WriteLine($"{book.Name};{book.Author};{book.Genre};{book.Year.ToString()};{book.Country}");
                }

                writer.Close();
            }
        }

        public void SaveUsersDb()
        {
            using (var writer = new StreamWriter(usersFilePath))
            {
                foreach (var user in Users)
                {
                    writer.WriteLine($"{user.Username};{user.PasswordHash};{user.Admin.ToString()}");
                }

                writer.Close();
            }
        }

        public void SaveDb()
        {
            SaveBooksDb();
            SaveUsersDb();
        }
    }
}
