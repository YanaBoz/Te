using System;

namespace Web_Library.Models
{
    public class Book
    {
        public int Id { get; set; }
        public string ISBN { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Genre { get; set; } = string.Empty;
        public int GenreID { get; set; }
        public string Description { get; set; } = string.Empty;
        public DateTime? BorrowedAt { get; set; }
        public DateTime? ReturnBy { get; set; }
        public string? ImageUrl { get; set; }
        public int Quantity { get; set; } = 0;
        public bool IsNotified { get; set; } = false;
        public int AuthorID { get; set; }
        public Author? Author { get; set; }
        public Genre? GenreNavigation { get; set; }
    }
}
