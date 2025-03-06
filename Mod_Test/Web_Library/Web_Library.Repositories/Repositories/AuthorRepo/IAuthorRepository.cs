using Web_Library.Models;

namespace Web_Library.Repositories
{
    public interface IAuthorRepository
    {
        Task<IEnumerable<Author>> GetAllAsync(CancellationToken cancellationToken);
        Task<(IEnumerable<Author> Authors, int TotalCount)> GetPaginatedAuthorsAsync(int pageNumber, int pageSize, CancellationToken cancellationToken);
        Task<Author?> GetByIdAsync(int id, CancellationToken cancellationToken);
        Task AddAsync(Author author, CancellationToken cancellationToken);
        Task UpdateAsync(Author author, CancellationToken cancellationToken);
        Task DeleteAsync(int id, CancellationToken cancellationToken);
        Task<int> GetIdByNameAsync(string firstName, string lastName, CancellationToken cancellationToken);
        Task<IEnumerable<Book>> GetBooksByAuthorIdAsync(int authorId, CancellationToken cancellationToken);
    }
}
