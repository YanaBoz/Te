using Web_Library.DTOs;

namespace Web_Library.Services
{
    public interface IBookService
    {
        Task<IEnumerable<BookDto>> GetAllAsync();
        Task<(IEnumerable<BookDto> Books, int TotalCount)> GetPaginatedBooksAsync(int pageNumber, int pageSize, string? title = null);
        Task<BookDto?> GetByIdAsync(int id);
        Task AddAsync(BookDto bookDto);
        Task UpdateAsync(BookDto bookDto);
        Task DeleteAsync(int id);
        Task<bool> IssueBook(int bookId, string userId);
        Task<IEnumerable<BookDto>> GetOverdueBooksAsync();
        Task<IEnumerable<BookDto>> GetOverdueBooksForUserAsync(string userId);
    }
}
