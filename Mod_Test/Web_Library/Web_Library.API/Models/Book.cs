using System.ComponentModel.DataAnnotations;

namespace Web_Library.API.Models
{
    public class Book
    {
        [Key]
        public int Id { get; set; }

        [Required, MaxLength(13)]
        public string ISBN { get; set; } = string.Empty;

        [Required, MaxLength(200)]
        public string Title { get; set; } = string.Empty;

        [Required, MaxLength(100)]
        public string Genre {  get; set; } = string.Empty;
        public int GenreID { get; set; }
        
        public string Description { get; set; } = string.Empty;

        public DateTime? BorrowedAt { get; set; }
        public DateTime? ReturnBy { get; set; }

        public string? ImageUrl { get; set; }

        public int Quantity { get; set; } = 0;
        public bool IsNotified { get; set; } = false; // Для уведомлений

        // Связь с автором (внешний ключ)
        public int AuthorID { get; set; }
    }
}

