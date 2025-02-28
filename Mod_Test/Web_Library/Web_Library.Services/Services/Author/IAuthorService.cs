using Web_Library.DTOs;

namespace Web_Library.Services
{
    public interface IAuthorService
    {
        Task<IEnumerable<AuthorDto>> GetAllAsync();
        Task<AuthorDto?> GetByIdAsync(int id);
        Task AddAsync(AuthorDto authorDto);
        Task UpdateAsync(AuthorDto authorDto);
        Task DeleteAsync(int id);
    }
}
