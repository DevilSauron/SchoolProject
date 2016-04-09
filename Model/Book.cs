using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkolniProjekt.Model
{
    public class Book
    {
        public string Name { get; set; }
        public string Author { get; set; }
        public string Genre { get; set; }
        public int Year { get; set; }
        public string Country { get; set; }

        public Book(string name, string author, string genre, int year, string country)
        {
            Name = name;
            Author = author;
            Genre = genre;
            Year = year;
            Country = country;
        }
    }
}
