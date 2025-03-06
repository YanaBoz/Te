using Web_Library.Models;

namespace Web_Library.Repositories
{
    public interface IGenreRepository
    {
        Task<Genre?> GetByIdAsync(int id, CancellationToken cancellationToken);
        Task<bool> ExistsAsync(int id, CancellationToken cancellationToken);
        Task<List<Genre>> GetAllAsync( CancellationToken cancellationToken);
        Task<int> GetIdByNameAsync(string name, CancellationToken cancellationToken);
    }
}
