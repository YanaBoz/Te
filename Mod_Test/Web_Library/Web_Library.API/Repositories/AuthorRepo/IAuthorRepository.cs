using Web_Library.API.Models;

namespace Web_Library.API.Repositories.AuthorRepo
{
    public interface IAuthorRepository
    {
        Task<(IEnumerable<Author> Authors, int TotalCount)> GetPaginatedAuthorsAsync(int pageNumber, int pageSize);
        Task<IEnumerable<Author>> GetAllAsync();
        Task<Author> GetByIdAsync(int id);
        Task AddAsync(Author author);
        Task UpdateAsync(Author author);
        Task<string?> GetAuthorFullName(int authorId);
        Task DeleteAsync(int id);
        Task<IEnumerable<Book>> GetBooksByAuthorIdAsync(int authorId);
        Task<bool> ExistsAsync(int authorId);  // Add ExistsAsync method definition
    }
}
