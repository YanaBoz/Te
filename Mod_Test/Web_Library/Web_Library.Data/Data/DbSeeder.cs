using Web_Library.Data;
using Web_Library.Models;

public static class DbSeeder
{
    public static void Seed(AppDbContext context)
    {
        if (!context.Genres.Any())
        {
            Console.WriteLine("Seeding Genres...");
            var genres = new List<Genre>
            {
                new Genre { Name = "Fantasy" },
                new Genre { Name = "Dystopian" },
                new Genre { Name = "Sci-Fi" },
            };
            context.Genres.AddRange(genres);
            context.SaveChanges();
            Console.WriteLine("Genres seeded.");
        }

        if (!context.Authors.Any())
        {
            Console.WriteLine("Seeding Authors...");
            var authors = new List<Author>
            {
                new Author
                {
                    FirstName = "J.K.",
                    LastName = "Rowling",
                    BirthDate = new DateTime(1965, 7, 31),
                    Country = "United Kingdom",
                    Books = new List<Book>
                    {
                        new Book
                        {
                            ISBN = "9780747532743",
                            Title = "Harry Potter and the Philosopher's Stone",
                            Genre = context.Genres.FirstOrDefault(g => g.Name == "Fantasy"),
                            Quantity = 5
                        }
                    }
                },
                new Author
                {
                    FirstName = "George",
                    LastName = "Orwell",
                    BirthDate = new DateTime(1903, 6, 25),
                    Country = "United Kingdom",
                    Books = new List<Book>
                    {
                        new Book
                        {
                            ISBN = "9780451524935",
                            Title = "1984",
                            Genre = context.Genres.FirstOrDefault(g => g.Name == "Dystopian"),
                            Quantity = 3
                        }
                    }
                }
            };

            context.Authors.AddRange(authors);
            context.SaveChanges();
            Console.WriteLine("Authors seeded.");
        }

        if (!context.Users.Any())
        {
            Console.WriteLine("Seeding Users...");
            var user = new User
            {
                Username = "john_doe",
                FullName = "John Doe",
                PasswordHash = "XV4rRarEADlYIKLdZCHWnSGJNagZXV17+5hbS7rmEM8=", // Esenin_04
                Role = "User"
            };

            var adminUser = new User
            {
                Username = "admin",
                FullName = "Administrator",
                PasswordHash = "XV4rRarEADlYIKLdZCHWnSGJNagZXV17+5hbS7rmEM8=", // Esenin_04
                Role = "Admin"
            };

            context.Users.Add(user);
            context.Users.Add(adminUser);
            context.SaveChanges();
            Console.WriteLine("Users seeded.");
        }
    }
}
