using System.Threading.Tasks;
using Web_Library.API.Models;

namespace Web_Library.API.Repositories.GenreRepo
{
    public interface IGenreRepository
    {
        Task<Genre> GetByIdAsync(int genreId); // Method to get genre by ID
        Task<bool> ExistsAsync(int genreId); // Check if genre exists
        Task<List<Genre>> GetAllAsync();
    }
}
