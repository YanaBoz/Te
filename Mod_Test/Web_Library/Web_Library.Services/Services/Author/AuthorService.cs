using Web_Library.DTOs;
using Web_Library.Models;
using Web_Library.Repositories;
using Mapster;

namespace Web_Library.Services
{
    public class AuthorService : IAuthorService
    {
        private readonly IAuthorRepository _authorRepository;

        public AuthorService(IAuthorRepository authorRepository)
        {
            _authorRepository = authorRepository;
        }

        public async Task<IEnumerable<AuthorDto>> GetAllAsync(CancellationToken cancellationToken)
        {
            var authors = await _authorRepository.GetAllAsync(cancellationToken);
            return authors.Adapt<IEnumerable<AuthorDto>>();
        }

        public async Task<AuthorDto?> GetByIdAsync(int id, CancellationToken cancellationToken)
        {
            var author = await _authorRepository.GetByIdAsync(id, cancellationToken);
            if (author == null) return null;

            return author.Adapt<AuthorDto>();
        }

        public async Task AddAsync(AuthorDto authorDto, CancellationToken cancellationToken)
        {
            var author = authorDto.Adapt<Author>();
            await _authorRepository.AddAsync(author, cancellationToken);
        }

        public async Task UpdateAsync(AuthorDto authorDto, CancellationToken cancellationToken)
        {
            var author = await _authorRepository.GetByIdAsync(authorDto.Id, cancellationToken);
            if (author == null)
                throw new Exception("Author not found");

            authorDto.Adapt(author);
            await _authorRepository.UpdateAsync(author, cancellationToken);
        }

        public async Task DeleteAsync(int id, CancellationToken cancellationToken)
        {
            await _authorRepository.DeleteAsync(id, cancellationToken);
        }
    }
}
