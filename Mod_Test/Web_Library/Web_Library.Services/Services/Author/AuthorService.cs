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

        public async Task<IEnumerable<AuthorDto>> GetAllAsync()
        {
            var authors = await _authorRepository.GetAllAsync();
            return authors.Adapt<IEnumerable<AuthorDto>>();
        }

        public async Task<AuthorDto?> GetByIdAsync(int id)
        {
            var author = await _authorRepository.GetByIdAsync(id);
            if (author == null) return null;

            return author.Adapt<AuthorDto>();
        }

        public async Task AddAsync(AuthorDto authorDto)
        {
            var author = authorDto.Adapt<Author>();
            await _authorRepository.AddAsync(author);
        }

        public async Task UpdateAsync(AuthorDto authorDto)
        {
            var author = await _authorRepository.GetByIdAsync(authorDto.Id);
            if (author == null)
                throw new Exception("Author not found");

            authorDto.Adapt(author);
            await _authorRepository.UpdateAsync(author);
        }

        public async Task DeleteAsync(int id)
        {
            await _authorRepository.DeleteAsync(id);
        }
    }
}
