using Microsoft.EntityFrameworkCore;
using Web_Library.Data;
using Web_Library.Models;

namespace Web_Library.Repositories
{
    public class AuthorRepository : IAuthorRepository
    {
        private readonly AppDbContext _context;

        public AuthorRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Author>> GetAllAsync(CancellationToken cancellationToken)
        {
            return await _context.Authors.Include(a => a.Books).ToListAsync(cancellationToken);
        }

        public async Task<(IEnumerable<Author> Authors, int TotalCount)> GetPaginatedAuthorsAsync(int pageNumber, int pageSize, CancellationToken cancellationToken)
        {
            int totalCount = await _context.Authors.CountAsync(cancellationToken);
            var authors = await _context.Authors
                .Include(a => a.Books)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);
            return (authors, totalCount);
        }

        public async Task<Author?> GetByIdAsync(int id, CancellationToken cancellationToken)
        {
            return await _context.Authors
                .AsNoTracking()
                .Include(a => a.Books)
                .FirstOrDefaultAsync(a => a.Id == id, cancellationToken);
        }

        public async Task AddAsync(Author author, CancellationToken cancellationToken)
        {
            await _context.Authors.AddAsync(author, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task UpdateAsync(Author author, CancellationToken cancellationToken)
        {
            _context.Authors.Update(author);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task DeleteAsync(int id, CancellationToken cancellationToken)
        {
            var author = await _context.Authors
                .FirstOrDefaultAsync(a => a.Id == id, cancellationToken);

            if (author != null)
            {
                _context.Entry(author).State = EntityState.Detached; 
                _context.Authors.Remove(author);
                await _context.SaveChangesAsync(cancellationToken);
            }
        }



        public async Task<int> GetIdByNameAsync(string firstName, string lastName, CancellationToken cancellationToken)
        {
            var author = await _context.Authors.FirstOrDefaultAsync(a => a.FirstName == firstName && a.LastName == lastName, cancellationToken);
            return author?.Id ?? 0;
        }

        public async Task<IEnumerable<Book>> GetBooksByAuthorIdAsync(int authorId, CancellationToken cancellationToken)
        {
            var author = await _context.Authors
                .Include(a => a.Books)
                .FirstOrDefaultAsync(a => a.Id == authorId, cancellationToken);

            return author?.Books ?? Enumerable.Empty<Book>();
        }
    }
}