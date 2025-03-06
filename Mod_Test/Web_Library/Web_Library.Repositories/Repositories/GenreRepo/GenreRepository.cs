using Microsoft.EntityFrameworkCore;
using Web_Library.Data;
using Web_Library.Models;

namespace Web_Library.Repositories
{
    public class GenreRepository : IGenreRepository
    {
        private readonly AppDbContext _context;
        public GenreRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Genre?> GetByIdAsync(int id, CancellationToken cancellationToken)
        {
            return await _context.Genres.FindAsync(new object[] { id }, cancellationToken);
        }

        public async Task<bool> ExistsAsync(int id, CancellationToken cancellationToken)
        {
            return await _context.Genres.AnyAsync(g => g.Id == id, cancellationToken);
        }

        public async Task<List<Genre>> GetAllAsync(CancellationToken cancellationToken)
        {
            return await _context.Genres.ToListAsync();
        }

        public async Task<int> GetIdByNameAsync(string name, CancellationToken cancellationToken)
        {
            var genre = await _context.Genres.FirstOrDefaultAsync(g => g.Name == name, cancellationToken);
            return genre?.Id ?? 0;
        }
    }
}
