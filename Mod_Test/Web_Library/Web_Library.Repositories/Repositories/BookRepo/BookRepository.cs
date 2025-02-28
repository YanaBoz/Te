using Microsoft.EntityFrameworkCore;
using Web_Library.Models;
using Web_Library.Data;

namespace Web_Library.Repositories
{
    public class BookRepository : IBookRepository
    {
        private readonly AppDbContext _context;
        public BookRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Book>> GetAllAsync()
        {
            return await _context.Books
                .Include(b => b.Author)
                .Include(b => b.GenreNavigation)
                .ToListAsync();
        }

        public async Task<(IEnumerable<Book> Books, int TotalCount)> GetPaginatedBooksAsync(int pageNumber, int pageSize, string? title = null)
        {
            var query = _context.Books.AsQueryable();
            if (!string.IsNullOrEmpty(title))
            {
                query = query.Where(b => b.Title.Contains(title));
            }
            int totalCount = await query.CountAsync();
            var books = await query.Skip((pageNumber - 1) * pageSize)
                                   .Take(pageSize)
                                   .Include(b => b.Author)
                                   .Include(b => b.GenreNavigation)
                                   .ToListAsync();
            return (books, totalCount);
        }

        public async Task<Book?> GetByIdAsync(int id)
        {
            return await _context.Books
                .Include(b => b.Author)
                .Include(b => b.GenreNavigation)
                .FirstOrDefaultAsync(b => b.Id == id);
        }

        public async Task<Book?> GetByIsbnAsync(string isbn)
        {
            return await _context.Books
                .Include(b => b.Author)
                .Include(b => b.GenreNavigation)
                .FirstOrDefaultAsync(b => b.ISBN == isbn);
        }

        public async Task AddAsync(Book book)
        {
            await _context.Books.AddAsync(book);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Book book)
        {
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
                return false;
            if (user.BorrowedBooks.Any(b => b.Id == book.Id))
                return false;
            book.Quantity--;
            book.BorrowedAt = DateTime.Now;
            book.ReturnBy = DateTime.Now.AddDays(14);
            user.BorrowedBooks.Add(book);
            await UpdateAsync(book);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<Book>> GetOverdueBooksAsync()
        {
            return await _context.Books
                .Where(b => b.ReturnBy < DateTime.Now)
                .Include(b => b.Author)
                .Include(b => b.GenreNavigation)
                .ToListAsync();
        }

        public async Task<IEnumerable<Book>> GetOverdueBooksForUserAsync(string userId)
        {
            var user = await _context.Users
                .Include(u => u.BorrowedBooks)
                .ThenInclude(b => b.Author)
                .Include(u => u.BorrowedBooks)
                .ThenInclude(b => b.GenreNavigation)
                .FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null)
                return Enumerable.Empty<Book>();
            return user.BorrowedBooks.Where(b => b.ReturnBy < DateTime.Now).ToList();
        }

        public async Task DeleteBooksByAuthorIdAsync(int authorId)
        {
            var books = await _context.Books.Where(b => b.AuthorID == authorId).ToListAsync();
            _context.Books.RemoveRange(books);
            await _context.SaveChangesAsync();
        }
    }
}
