using Microsoft.EntityFrameworkCore;
using Web_Library.API.Models;

namespace Web_Library.API.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Book> Books { get; set; }
        public DbSet<Author> Authors { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }

        public DbSet<Genre> Genres { get; set; } 

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
    }
}
