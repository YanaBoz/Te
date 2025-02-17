﻿using Web_Library.API.Models;

namespace Web_Library.API.Data
{
    public static class DbSeeder
    {
        public static void Seed(AppDbContext context)
        {
            // Check if the database is already populated, if not, seed it
            if (!context.Authors.Any())
            {
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
                            GenreID = 1, // Assume this GenreID exists
                            Genre = "Fantasy",
                            Description = "A young wizard embarks on his first year at Hogwarts.",
                            Quantity = 5,
                            AuthorID = 1
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
                            GenreID = 2, // Assume this GenreID exists
                            Genre = "Dystopian",
                            Description = "A dystopian novel set in a totalitarian society.",
                            Quantity = 3,
                            AuthorID = 2
                        }
                    }
                }
            };

                context.Authors.AddRange(authors);
                context.SaveChanges();
            }

            // Check if Genres exist, if not, create some
            if (!context.Genres.Any())
            {
                var genres = new List<Genre>
            {
                new Genre { Name = "Fantasy" },
                new Genre { Name = "Dystopian" },
                new Genre { Name = "Sci-Fi" },
            };
                context.Genres.AddRange(genres);
                context.SaveChanges();
            }

            // Check if Users exist, if not, create one
            if (!context.Users.Any())
            {
                var user = new User
                {
                    Username = "john_doe",
                    PasswordHash = "hashed_password", // Replace with actual hashed password
                    Role = "User"
                };
                context.Users.Add(user);
                context.SaveChanges();
            }
        }
    }
}
