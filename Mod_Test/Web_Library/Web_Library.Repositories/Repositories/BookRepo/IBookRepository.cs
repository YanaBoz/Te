using Web_Library.Models;

namespace Web_Library.Repositories
{
    public interface IBookRepository
    {
        Task<IEnumerable<Book>> GetAllAsync( CancellationToken cancellationToken);
        Task<(IEnumerable<Book> Books, int TotalCount)> GetPaginatedBooksAsync(int pageNumber, int pageSize, CancellationToken cancellationToken, string? title = null);
        Task<Book?> GetByIdAsync(int id, CancellationToken cancellationToken);
        Task<Book?> GetByIsbnAsync(string isbn, CancellationToken cancellationToken);
        Task AddAsync(Book book, CancellationToken cancellationToken);
        Task UpdateAsync(Book book, CancellationToken cancellationToken);
        Task DeleteAsync(int id, CancellationToken cancellationToken);
        Task<IEnumerable<Book>> GetOverdueBooksAsync(CancellationToken cancellationToken);
        Task<IEnumerable<Book>> GetOverdueBooksForUserAsync(string userId, CancellationToken cancellationToken);
    }
}
