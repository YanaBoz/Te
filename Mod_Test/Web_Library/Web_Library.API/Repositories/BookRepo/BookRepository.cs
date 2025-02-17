using Microsoft.EntityFrameworkCore;
using Web_Library.API.Data;
using Web_Library.API.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Web_Library.API.Repositories.GenreRepo;

namespace Web_Library.API.Repositories.BookRepo
{
    public class BookRepository : IBookRepository
    {
        private readonly AppDbContext _context;
        private readonly IGenreRepository _genreRepository; 

        public BookRepository(AppDbContext context, IGenreRepository genreRepository)
        {
            _context = context;
            _genreRepository = genreRepository; 
        }

        public async Task<IEnumerable<Book>> GetAllAsync()
        {
            return await _context.Books.ToListAsync();
        }

        public async Task<(IEnumerable<Book> Books, int TotalCount)> GetPaginatedBooksAsync(int? pageNumber = null, int? pageSize = null, string? title = null)
        {
            var query = _context.Books.AsQueryable();

            if (!string.IsNullOrEmpty(title))
                query = query.Where(b => b.Title.Contains(title));

            var totalCount = await query.CountAsync();

            // Если pageNumber или pageSize не заданы, возвращаем все книги
            if (!pageNumber.HasValue || !pageSize.HasValue)
            {
                var allBooks = await query.ToListAsync();
                return (allBooks, totalCount);
            }

            var books = await query.Skip((pageNumber.Value - 1) * pageSize.Value)
                                   .Take(pageSize.Value)
                                   .ToListAsync();

            // Включаем данные о жанре для каждой книги
            var booksWithGenres = books.Select(book => new Book
            {
                Id = book.Id,
                Title = book.Title,
                ISBN = book.ISBN,
                GenreID = book.GenreID,
                Description = book.Description,
                Quantity = book.Quantity,
                AuthorID = book.AuthorID,
                Genre = (_genreRepository.GetByIdAsync(book.GenreID))?.ToString() ?? "Unknown Genre"
            }).ToList();

            return (booksWithGenres, totalCount);
        }

        public async Task<Book> GetByIdAsync(int id)
        {
            return await _context.Books.FirstOrDefaultAsync(b => b.Id == id);
        }

        public async Task<Book> GetByIsbnAsync(string isbn)
        {
            return await _context.Books.FirstOrDefaultAsync(b => b.ISBN == isbn);
        }

        public async Task AddAsync(Book book)
        {
            var genreExists = await _genreRepository.ExistsAsync(book.GenreID);
            if (!genreExists)
            {
                throw new KeyNotFoundException("Жанр с данным ID не существует.");
            }

            await _context.Books.AddAsync(book);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Book book)
        {
            var genreExists = await _genreRepository.ExistsAsync(book.GenreID);
            if (!genreExists)
            {
                throw new KeyNotFoundException("Жанр с данным ID не существует.");
            }

            _context.Books.Update(book);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var book = await GetByIdAsync(id);
            if (book != null)
            {
                _context.Books.Remove(book);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> IssueBook(int bookId, string userId)
        {
            var book = await GetByIdAsync(bookId);
            var user = await _context.Users.Include(u => u.BorrowedBooks)
                                            .FirstOrDefaultAsync(u => u.Id == userId);

            if (book == null || book.Quantity <= 0 || user == null)
            {
                return false; // Книга не найдена, нет в наличии или пользователь не найден
            }

            // Проверяем, уже есть ли у пользователя эта книга
            if (user.BorrowedBooks.Any(b => b.Id == book.Id))
            {
                return false; // Пользователь уже взял эту книгу
            }

            // Логика выдачи книги пользователю
            book.Quantity--;
            book.BorrowedAt = DateTime.Now;
            book.ReturnBy = DateTime.Now.AddDays(14);

            user.BorrowedBooks.Add(book);

            await UpdateAsync(book);
            await _context.SaveChangesAsync(); // Сохранение изменений
            return true;
        }

        public async Task<IEnumerable<Book>> GetOverdueBooksAsync()
        {
            return await _context.Books
                .Where(b => b.ReturnBy < DateTime.Now)
                .ToListAsync();
        }

        public async Task DeleteBooksByAuthorIdAsync(int authorId)
        {
            var books = await _context.Books.Where(b => b.AuthorID == authorId).ToListAsync();
            _context.Books.RemoveRange(books);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Book>> GetOverdueBooksForUserAsync(string userId)
        {
            var user = await _context.Users.Include(u => u.BorrowedBooks)
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
            {
                return Enumerable.Empty<Book>();
            }

            return user.BorrowedBooks.Where(b => b.ReturnBy < DateTime.Now).ToList();
        }
    }
}
