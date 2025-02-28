using System;
using System.Collections.Generic;

namespace Web_Library.Models
{
    public class Author
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public DateTime BirthDate { get; set; }
        public string Country { get; set; } = string.Empty;
        public List<Book> Books { get; set; } = new();
    }
}
