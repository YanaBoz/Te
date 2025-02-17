using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Web_Library.API.Data;
using Web_Library.API.Models;
using Web_Library.API.Repositories.AuthorRepo;
using Xunit;

namespace Web_Library.Test
{
    public class AuthorRepositoryTests : IDisposable
    {
        private readonly AuthorRepository _repository;
        private readonly AppDbContext _context;

        public AuthorRepositoryTests()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // Unique database name
                .Options;

            _context = new AppDbContext(options);
            _repository = new AuthorRepository(_context);
        }

        [Fact]
        public async Task GetAllAsync_ReturnsAllAuthors()
        {
            var authors = new List<Author>
            {
                new Author { Id = 1, FirstName = "John", LastName = "Doe", BirthDate = new DateTime(1980, 1, 1), Country = "USA" },
                new Author { Id = 2, FirstName = "Jane", LastName = "Doe", BirthDate = new DateTime(1990, 1, 1), Country = "USA" }
            };

            await _context.Authors.AddRangeAsync(authors);
            await _context.SaveChangesAsync();

            var result = await _repository.GetAllAsync();
            Assert.Equal(2, result.Count());
        }

        [Fact]
        public async Task GetByIdAsync_ReturnsAuthor_WhenAuthorExists()
        {
            var author = new Author { Id = 1, FirstName = "John", LastName = "Doe", BirthDate = new DateTime(1980, 1, 1), Country = "USA" };
            await _context.Authors.AddAsync(author);
            await _context.SaveChangesAsync();

            var result = await _repository.GetByIdAsync(1);
            Assert.NotNull(result);
            Assert.Equal("John", result.FirstName);
        }

        [Fact]
        public async Task AddAsync_AddsAuthor()
        {
            var author = new Author { FirstName = "John", LastName = "Doe", BirthDate = new DateTime(1980, 1, 1), Country = "USA" };

            await _repository.AddAsync(author);
            var result = await _repository.GetAllAsync();
            Assert.Single(result);
        }

        [Fact]
        public async Task DeleteAsync_RemovesAuthor()
        {
            var author = new Author { Id = 1, FirstName = "John", LastName = "Doe", BirthDate = new DateTime(1980, 1, 1), Country = "USA" };
            await _context.Authors.AddAsync(author);
            await _context.SaveChangesAsync();

            await _repository.DeleteAsync(1);
            var result = await _repository.GetAllAsync();
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetBooksByAuthorIdAsync_ReturnsBooks_WhenBooksExist()
        {
            var author = new Author { Id = 1, FirstName = "John", LastName = "Doe", BirthDate = new DateTime(1980, 1, 1), Country = "USA", Books = new List<Book>() };
            var book1 = new Book { Id = 1, Title = "Book 1", AuthorID = 1 };
            var book2 = new Book { Id = 2, Title = "Book 2", AuthorID = 1 };

            await _context.Authors.AddAsync(author);
            await _context.Books.AddRangeAsync(book1, book2);
            await _context.SaveChangesAsync();

            var result = await _repository.GetBooksByAuthorIdAsync(1);
            Assert.Equal(2, result.Count());
        }

        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }
    }
}