using Web_Library.Models;

namespace Web_Library.Repositories
{
    public interface IBookRepository
    {
        Task<IEnumerable<Book>> GetAllAsync();
        Task<(IEnumerable<Book> Books, int TotalCount)> GetPaginatedBooksAsync(int pageNumber, int pageSize, string? title = null);
        Task<Book?> GetByIdAsync(int id);
        Task<Book?> GetByIsbnAsync(string isbn);
        Task AddAsync(Book book);
        Task UpdateAsync(Book book);
        Task DeleteAsync(int id);
        Task<bool> IssueBook(int bookId, string userId);
        Task<IEnumerable<Book>> GetOverdueBooksAsync();
        Task<IEnumerable<Book>> GetOverdueBooksForUserAsync(string userId);
        Task DeleteBooksByAuthorIdAsync(int authorId);
    }
}
