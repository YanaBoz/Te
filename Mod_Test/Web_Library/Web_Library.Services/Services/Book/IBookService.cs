using Web_Library.DTOs;

namespace Web_Library.Services
{
    public interface IBookService
    {
        Task<IEnumerable<BookDto>> GetAllAsync(CancellationToken cancellationToken);
        Task<(IEnumerable<BookDto> Books, int TotalCount)> GetPaginatedBooksAsync(int pageNumber, int pageSize, CancellationToken cancellationToken, string? title = null);
        Task<BookDto?> GetByIdAsync(int id, CancellationToken cancellationToken);
        Task AddAsync(BookDto bookDto, CancellationToken cancellationToken);
        Task UpdateAsync(BookDto bookDto, CancellationToken cancellationToken);
        Task DeleteAsync(int id, CancellationToken cancellationToken);
        Task<bool> IssueBook(int bookId, string userId, CancellationToken cancellationToken);
        Task<IEnumerable<BookDto>> GetOverdueBooksAsync(CancellationToken cancellationToken);
        Task<IEnumerable<BookDto>> GetOverdueBooksForUserAsync(string userId, CancellationToken cancellationToken);
        Task DeleteBooksByAuthorIdAsync(int authorId, CancellationToken cancellationToken);
    }
}
