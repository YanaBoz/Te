using Microsoft.EntityFrameworkCore;
using Web_Library.API.Data;
using Web_Library.API.Models;

namespace Web_Library.API.Repositories.AuthorRepo
{
    public class AuthorRepository : IAuthorRepository
    {
        private readonly AppDbContext _context;

        public AuthorRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Author>> GetAllAsync()
        {
            return await _context.Authors.ToListAsync();
        }

        public async Task<(IEnumerable<Author> Authors, int TotalCount)> GetPaginatedAuthorsAsync(int pageNumber, int pageSize)
        {
            var totalCount = await _context.Authors.CountAsync();
            var authors = await _context.Authors
                .Include(a => a.Books)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (authors, totalCount);
        }

        public async Task<Author> GetByIdAsync(int id)
        {
            return await _context.Authors
                .Include(a => a.Books)
                .FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task AddAsync(Author author)
        {
            await _context.Authors.AddAsync(author);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Author author)
        {
            _context.Authors.Update(author);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var author = await GetByIdAsync(id);
            if (author != null)
            {
                _context.Authors.Remove(author);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<string?> GetAuthorFullName(int authorId)
        {
            var author = await _context.Authors.FindAsync(authorId);
            return author != null ? $"{author.FirstName} {author.LastName}" : null;
        }

        public async Task<IEnumerable<Book>> GetBooksByAuthorIdAsync(int authorId)
        {
            var author = await GetByIdAsync(authorId);
            return author?.Books ?? new List<Book>();
        }

        public async Task<bool> ExistsAsync(int authorId)
        {
            return await _context.Authors.AnyAsync(a => a.Id == authorId);
        }
    }
}
