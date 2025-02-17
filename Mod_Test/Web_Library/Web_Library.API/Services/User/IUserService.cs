using System.Threading.Tasks;
using Web_Library.API.Models;

namespace Web_Library.API.Services
{
    public interface IUserService
    {
        Task<User> AuthenticateAsync(string username, string password);
        Task<User> GetUserByIdAsync(string userId);
        Task<List<Book>> GetBorrowedBooksAsync(string userId);
        Task<bool> RegisterAsync(User user, string password);
        Task<User> GetUserByUsernameAsync(string username);
    }
}
