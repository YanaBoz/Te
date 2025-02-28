namespace Web_Library.DTOs
{
    public class BookDto
    {
        public int Id { get; set; }
        public string ISBN { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Genre { get; set; } = string.Empty; 
        public string Description { get; set; } = string.Empty;
        public DateTime? BorrowedAt { get; set; }
        public DateTime? ReturnBy { get; set; }
        public string? ImageUrl { get; set; }
        public int Quantity { get; set; } = 0;
        public bool IsNotified { get; set; } = false;

        public int AuthorID { get; set; }
        public string AuthorName { get; set; } = string.Empty; 
    }
}
