using Web_Library.DTOs;
using Web_Library.Models;
using Web_Library.Repositories;
using Mapster;

namespace Web_Library.Services
{
    public class BookService : IBookService
    {
        private readonly IBookRepository _bookRepository;
        private readonly IGenreRepository _genreRepository;
        private readonly IUserRepository _userRepository;

        public BookService(IUserRepository userRepository, IBookRepository bookRepository, IGenreRepository genreRepository)
        {
            _bookRepository = bookRepository;
            _genreRepository = genreRepository;
            _userRepository = userRepository;
        }

        public async Task<IEnumerable<BookDto>> GetAllAsync(CancellationToken cancellationToken)
        {
            var books = await _bookRepository.GetAllAsync(cancellationToken);
            return books.Adapt<IEnumerable<BookDto>>();
        }

        public async Task<(IEnumerable<BookDto> Books, int TotalCount)> GetPaginatedBooksAsync(int pageNumber, int pageSize, CancellationToken cancellationToken, string? title = null)
        {
            var (books, totalCount) = await _bookRepository.GetPaginatedBooksAsync(pageNumber, pageSize, cancellationToken, title);
            var bookDtos = books.Adapt<IEnumerable<BookDto>>();
            return (bookDtos, totalCount);
        }

        public async Task<BookDto?> GetByIdAsync(int id, CancellationToken cancellationToken)
        {
            var book = await _bookRepository.GetByIdAsync(id, cancellationToken);
            return book?.Adapt<BookDto>();
        }

        public async Task AddAsync(BookDto bookDto, CancellationToken cancellationToken)
        {
            var book = bookDto.Adapt<Book>();
            book.GenreID = await _genreRepository.GetIdByNameAsync(bookDto.Genre, cancellationToken);
            await _bookRepository.AddAsync(book, cancellationToken);
        }

        public async Task UpdateAsync(BookDto bookDto, CancellationToken cancellationToken)
        {
            var book = await _bookRepository.GetByIdAsync(bookDto.Id, cancellationToken);
            if (book == null)
                throw new Exception("Book not found");
            bookDto.Adapt(book);
            book.GenreID = await _genreRepository.GetIdByNameAsync(bookDto.Genre, cancellationToken);
            await _bookRepository.UpdateAsync(book, cancellationToken);
        }

        public async Task DeleteAsync(int id, CancellationToken cancellationToken)
        {
            await _bookRepository.DeleteAsync(id, cancellationToken);
        }

        public async Task<bool> IssueBook(int bookId, string userId, CancellationToken cancellationToken)
        {
            var book = await _bookRepository.GetByIdAsync(bookId, cancellationToken);
            if (book == null)
                throw new Exception("Book not found");
            if (book.Quantity <= 0)
                throw new Exception("No copies available");

            var user = await _userRepository.GetByIdAsync(userId, cancellationToken);
            if (user == null)
                throw new Exception("User not found");

            if (user.BorrowedBooks.Any(b => b.Id == book.Id))
                throw new Exception("User has already borrowed this book");

            book.Quantity--;
            book.BorrowedAt = DateTime.Now;
            book.ReturnBy = DateTime.Now.AddDays(14);

            user.BorrowedBooks.Add(book);

            await _bookRepository.UpdateAsync(book, cancellationToken);
            return true;
        }

        public async Task<IEnumerable<BookDto>> GetOverdueBooksAsync(CancellationToken cancellationToken)
        {
            var books = await _bookRepository.GetOverdueBooksAsync(cancellationToken);
            return books.Adapt<IEnumerable<BookDto>>();
        }

        public async Task<IEnumerable<BookDto>> GetOverdueBooksForUserAsync(string userId, CancellationToken cancellationToken)
        {
            var books = await _bookRepository.GetOverdueBooksForUserAsync(userId, cancellationToken);
            return books.Adapt<IEnumerable<BookDto>>();
        }
        public async Task DeleteBooksByAuthorIdAsync(int authorId, CancellationToken cancellationToken)
        {
            var books = await _bookRepository.GetAllAsync(cancellationToken);
            var booksByAuthor = books.Where(b => b.AuthorID == authorId).ToList();

            if (!booksByAuthor.Any())
                throw new Exception("No books found for this author");

            foreach (var book in booksByAuthor)
            {
                await _bookRepository.DeleteAsync(book.Id, cancellationToken);
            }
        }
    }
}
