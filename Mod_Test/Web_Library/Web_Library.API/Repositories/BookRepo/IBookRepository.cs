using System.Collections.Generic;
using System.Threading.Tasks;
using Web_Library.API.Models;

namespace Web_Library.API.Repositories.BookRepo
{
    public interface IBookRepository
    {
        Task<(IEnumerable<Book> Books, int TotalCount)> GetPaginatedBooksAsync(int? pageNumber = null, int? pageSize = null, string? title = null);
        Task<IEnumerable<Book>> GetAllAsync();
        Task<Book> GetByIdAsync(int id);
        Task<Book> GetByIsbnAsync(string isbn);
        Task AddAsync(Book book);
        Task UpdateAsync(Book book);
        Task DeleteAsync(int id);
        Task<IEnumerable<Book>> GetOverdueBooksAsync();
        Task<IEnumerable<Book>> GetOverdueBooksForUserAsync(string userId);
        Task DeleteBooksByAuthorIdAsync(int authorId);
        Task<bool> IssueBook(int bookId, string userId); // Метод для выдачи книги
    }
}