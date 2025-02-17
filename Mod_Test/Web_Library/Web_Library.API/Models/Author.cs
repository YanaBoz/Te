using System.ComponentModel.DataAnnotations;

namespace Web_Library.API.Models
{
    public class Author
    {
        [Key]
        public int Id { get; set; }

        [Required, MaxLength(50)]
        public string FirstName { get; set; } = string.Empty;

        [Required, MaxLength(50)]
        public string LastName { get; set; } = string.Empty;

        [Required]
        public DateTime BirthDate { get; set; }

        [Required, MaxLength(50)]
        public string Country { get; set; } = string.Empty;

        public List<Book> Books { get; set; } = new();
    }
}
