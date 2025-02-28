using Web_Library.Models;

namespace Web_Library.Repositories
{
    public interface IGenreRepository
    {
        Task<Genre?> GetByIdAsync(int id);
        Task<bool> ExistsAsync(int id);
        Task<List<Genre>> GetAllAsync();
        Task<int> GetIdByNameAsync(string name);
    }
}
