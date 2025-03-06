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

        public async Task<IEnumerable<Book>> GetAllAsync(CancellationToken cancellationToken)
        {
            return await _context.Books
                .Include(b => b.Author)
                .Include(b => b.GenreNavigation)
                .ToListAsync();
        }

        public async Task<(IEnumerable<Book> Books, int TotalCount)> GetPaginatedBooksAsync(int pageNumber, int pageSize, CancellationToken cancellationToken, string? title = null)
        {
            var query = _context.Books.AsQueryable();
            if (!string.IsNullOrEmpty(title))
            {
                query = query.Where(b => b.Title.Contains(title));
            }

            int totalCount = await query.CountAsync(cancellationToken);
            var books = await query.Skip((pageNumber - 1) * pageSize)
                                   .Take(pageSize)
                                   .Include(b => b.Author)
                                   .Include(b => b.GenreNavigation)
                                   .ToListAsync(cancellationToken); 
            return (books, totalCount);
        }


        public async Task<Book?> GetByIdAsync(int id, CancellationToken cancellationToken)
        {
            return await _context.Books
                .Include(b => b.Author)
                .Include(b => b.GenreNavigation)
                .FirstOrDefaultAsync(b => b.Id == id, cancellationToken);
        }

        public async Task<Book?> GetByIsbnAsync(string isbn, CancellationToken cancellationToken)
        {
            return await _context.Books
                .Include(b => b.Author)
                .Include(b => b.GenreNavigation)
                .FirstOrDefaultAsync(b => b.ISBN == isbn, cancellationToken);
        }

        public async Task AddAsync(Book book, CancellationToken cancellationToken)
        {
            await _context.Books.AddAsync(book, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task UpdateAsync(Book book, CancellationToken cancellationToken)
        {
            _context.Books.Update(book);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task DeleteAsync(int id, CancellationToken cancellationToken)
        {
            var book = await GetByIdAsync(id, cancellationToken);
            if (book != null)
            {
                _context.Books.Remove(book);
                await _context.SaveChangesAsync(cancellationToken);
            }
        }

        public async Task<IEnumerable<Book>> GetOverdueBooksAsync(CancellationToken cancellationToken)
        {
            return await _context.Books
                .Where(b => b.ReturnBy < DateTime.Now)
                .Include(b => b.Author)
                .Include(b => b.GenreNavigation)
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<Book>> GetOverdueBooksForUserAsync(string userId, CancellationToken cancellationToken)
        {
            var user = await _context.Users
                .Include(u => u.BorrowedBooks)
                .ThenInclude(b => b.Author)
                .Include(u => u.BorrowedBooks)
                .ThenInclude(b => b.GenreNavigation)
                .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);
            if (user == null)
                return Enumerable.Empty<Book>();
            return user.BorrowedBooks.Where(b => b.ReturnBy < DateTime.Now).ToList();
        }

    }
}
