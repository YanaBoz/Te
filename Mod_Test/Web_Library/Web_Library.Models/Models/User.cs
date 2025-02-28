using System;
using System.Collections.Generic;

namespace Web_Library.Models
{
    public class User
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Username { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string? Role { get; set; } = "User";
        public ICollection<Book>? BorrowedBooks { get; set; } = new List<Book>();
    }
}
