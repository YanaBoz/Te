using System.ComponentModel.DataAnnotations;

namespace Web_Library.API.Models
{
    public class User
    {
        public string Id { get; set; } = Guid.NewGuid().ToString(); 

        [Required]
        public string Username { get; set; } = string.Empty;

        [Required]
        public string FullName { get; set; } = string.Empty;

        public string PasswordHash { get; set; } = string.Empty; 

        public string? Role { get; set; } = "User";

        public List<Book>? BorrowedBooks { get; set; } = new();
    }
}
