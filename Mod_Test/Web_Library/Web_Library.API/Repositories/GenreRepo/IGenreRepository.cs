using System.Threading.Tasks;
using Web_Library.API.Models;

namespace Web_Library.API.Repositories.GenreRepo
{
    public interface IGenreRepository
    {
        Task<Genre> GetByIdAsync(int genreId); 
        Task<bool> ExistsAsync(int genreId); 
        Task<List<Genre>> GetAllAsync();
    }
}
