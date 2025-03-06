using Web_Library.DTOs;

namespace Web_Library.Services
{
    public interface IAuthorService
    {
        Task<IEnumerable<AuthorDto>> GetAllAsync(CancellationToken cancellationToken);
        Task<AuthorDto?> GetByIdAsync(int id, CancellationToken cancellationToken);
        Task AddAsync(AuthorDto authorDto, CancellationToken cancellationToken);
        Task UpdateAsync(AuthorDto authorDto, CancellationToken cancellationToken);
        Task DeleteAsync(int id, CancellationToken cancellationToken);
    }
}
