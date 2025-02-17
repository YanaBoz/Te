using Microsoft.EntityFrameworkCore;
using Web_Library.API.Data;
using Web_Library.API.Models;
using System.Threading.Tasks;

namespace Web_Library.API.Repositories.GenreRepo
{
    public class GenreRepository : IGenreRepository
    {
        private readonly AppDbContext _context;

        public GenreRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Genre> GetByIdAsync(int genreId)
        {
            return await _context.Genres.FirstOrDefaultAsync(g => g.Id == genreId);
        }

        public async Task<bool> ExistsAsync(int genreId)
        {
            return await _context.Genres.AnyAsync(g => g.Id == genreId);
        }
        public async Task<List<Genre>> GetAllAsync()
        {
            return await _context.Genres.ToListAsync();
        }
    }
}
