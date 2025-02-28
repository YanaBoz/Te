using Web_Library.DTOs;
using Web_Library.Models;
using Web_Library.Repositories;
using Mapster;

namespace Web_Library.Services
{
    public class BookService : IBookService
    {
        private readonly IBookRepository _bookRepository;
        private readonly IAuthorRepository _authorRepository;
        private readonly IGenreRepository _genreRepository;

        public BookService(IBookRepository bookRepository, IAuthorRepository authorRepository, IGenreRepository genreRepository)
        {
            _bookRepository = bookRepository;
            _authorRepository = authorRepository;
            _genreRepository = genreRepository;
        }

        public async Task<IEnumerable<BookDto>> GetAllAsync()
        {
            var books = await _bookRepository.GetAllAsync();
            return books.Adapt<IEnumerable<BookDto>>();
        }

        public async Task<(IEnumerable<BookDto> Books, int TotalCount)> GetPaginatedBooksAsync(int pageNumber, int pageSize, string? title = null)
        {
            var (books, totalCount) = await _bookRepository.GetPaginatedBooksAsync(pageNumber, pageSize, title);
            var bookDtos = books.Adapt<IEnumerable<BookDto>>();
            return (bookDtos, totalCount);
        }

        public async Task<BookDto?> GetByIdAsync(int id)
        {
            var book = await _bookRepository.GetByIdAsync(id);
            return book?.Adapt<BookDto>();
        }

        public async Task AddAsync(BookDto bookDto)
        {
            var book = bookDto.Adapt<Book>();
            book.GenreID = await _genreRepository.GetIdByNameAsync(bookDto.Genre);
            await _bookRepository.AddAsync(book);
        }

        public async Task UpdateAsync(BookDto bookDto)
        {
            var book = await _bookRepository.GetByIdAsync(bookDto.Id);
            if (book == null)
                throw new Exception("Book not found");
            bookDto.Adapt(book);
            book.GenreID = await _genreRepository.GetIdByNameAsync(bookDto.Genre);
            await _bookRepository.UpdateAsync(book);
        }

        public async Task DeleteAsync(int id)
        {
            await _bookRepository.DeleteAsync(id);
        }

        public async Task<bool> IssueBook(int bookId, string userId)
        {
            return await _bookRepository.IssueBook(bookId, userId);
        }

        public async Task<IEnumerable<BookDto>> GetOverdueBooksAsync()
        {
            var books = await _bookRepository.GetOverdueBooksAsync();
            return books.Adapt<IEnumerable<BookDto>>();
        }

        public async Task<IEnumerable<BookDto>> GetOverdueBooksForUserAsync(string userId)
        {
            var books = await _bookRepository.GetOverdueBooksForUserAsync(userId);
            return books.Adapt<IEnumerable<BookDto>>();
        }
    }
}
