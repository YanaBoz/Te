using Web_Library.Models;

namespace Web_Library.Repositories
{
    public interface IAuthorRepository
    {
        Task<IEnumerable<Author>> GetAllAsync();
        Task<(IEnumerable<Author> Authors, int TotalCount)> GetPaginatedAuthorsAsync(int pageNumber, int pageSize);
        Task<Author?> GetByIdAsync(int id);
        Task AddAsync(Author author);
        Task UpdateAsync(Author author);
        Task DeleteAsync(int id);
        Task<int> GetIdByNameAsync(string firstName, string lastName);
        Task<IEnumerable<Book>> GetBooksByAuthorIdAsync(int authorId);
    }
}
