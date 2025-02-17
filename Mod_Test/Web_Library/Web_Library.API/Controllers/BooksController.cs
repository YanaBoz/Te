using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Web_Library.API.Models;
using Web_Library.API.Repositories.AuthorRepo;
using Web_Library.API.Repositories.BookRepo;
using Web_Library.API.Repositories.GenreRepo;
using Web_Library.API.Services;

namespace Web_Library.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BooksController : ControllerBase
    {
        private readonly IBookRepository _bookRepository;
        private readonly IUserService _userService;
        private readonly IAuthorRepository _authorRepository;
        private readonly IGenreRepository _genreRepository;  // Added Genre Repository

        public BooksController(
            IBookRepository bookRepository,
            IUserService userService,
            IAuthorRepository authorRepository,
            IGenreRepository genreRepository) // Injecting genre repository
        {
            _bookRepository = bookRepository;
            _userService = userService;
            _authorRepository = authorRepository;
            _genreRepository = genreRepository;  // Assigning genre repository
        }

        // Получить все книги
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Book>>> GetBooks([FromQuery] string? title, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var (books, totalCount) = await _bookRepository.GetPaginatedBooksAsync(pageNumber, pageSize, title);

            if (!books.Any())
            {
                return NotFound("Книги не найдены.");
            }

            // Подгружаем информацию о жанре и авторах
            var booksWithAuthorsAndGenres = books.Select(book => new
            {
                book.Id,
                book.Title,
                GenreName = _genreRepository.GetByIdAsync(book.GenreID).Result?.Name, // Corrected method name here
                book.Quantity,
                AuthorName = _authorRepository.GetAuthorFullName(book.AuthorID).Result // Получаем ФИО автора
            });

            return Ok(new { TotalCount = totalCount, Books = booksWithAuthorsAndGenres });
        }

        // Получить книгу по ID
        [HttpGet("{id}")]
        public async Task<ActionResult<Book>> GetBook(int id)
        {
            var book = await _bookRepository.GetByIdAsync(id);
            if (book == null)
            {
                return NotFound("Книга не найдена.");
            }
            return Ok(book);
        }

        // Добавить новую книгу
        [HttpPost]
        public async Task<ActionResult<Book>> CreateBook([FromBody] Book book)
        {
            // Проверка на существование автора и жанра
            var authorExists = await _authorRepository.ExistsAsync(book.AuthorID);
            if (!authorExists)
            {
                return BadRequest("Автор с данным ID не существует.");
            }

            var genreExists = await _genreRepository.ExistsAsync(book.GenreID);
            if (!genreExists)
            {
                return BadRequest("Жанр с данным ID не существует.");
            }

            await _bookRepository.AddAsync(book);
            return CreatedAtAction(nameof(GetBook), new { id = book.Id }, book);
        }

        // Обновить существующую книгу
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateBook(int id, [FromBody] Book book)
        {
            if (id != book.Id)
            {
                return BadRequest("ID книги не совпадает.");
            }

            // Проверка на существование автора и жанра
            var authorExists = await _authorRepository.ExistsAsync(book.AuthorID);
            if (!authorExists)
            {
                return BadRequest("Автор с данным ID не существует.");
            }

            var genreExists = await _genreRepository.ExistsAsync(book.GenreID);
            if (!genreExists)
            {
                return BadRequest("Жанр с данным ID не существует.");
            }

            await _bookRepository.UpdateAsync(book);
            return NoContent();
        }

        // Удалить книгу
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBook(int id)
        {
            var book = await _bookRepository.GetByIdAsync(id);
            if (book == null)
            {
                return NotFound("Книга не найдена.");
            }

            await _bookRepository.DeleteAsync(id);
            return NoContent();
        }

        [HttpPost("issue/{bookId}")]
        public async Task<IActionResult> IssueBook(int bookId)
        {
            var username = User.FindFirstValue(ClaimTypes.NameIdentifier); // Извлечение никнейма
            if (string.IsNullOrEmpty(username))
            {
                return Unauthorized("Пользователь не аутентифицирован.");
            }

            // Найти пользователя по никнейму
            var user = await _userService.GetUserByUsernameAsync(username);
            if (user == null)
            {
                return NotFound("Пользователь не найден.");
            }

            var result = await _bookRepository.IssueBook(bookId, user.Id);
            if (!result)
            {
                return BadRequest("Книга не доступна для выдачи.");
            }

            return Ok("Книга успешно выдана!");
        }

        [HttpGet("user-overdue")]
        public async Task<ActionResult<IEnumerable<Book>>> GetUserOverdueBooks()
        {
            var username = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(username))
            {
                return Unauthorized("Пользователь не аутентифицирован.");
            }

            var user = await _userService.GetUserByUsernameAsync(username);
            if (user == null)
            {
                return NotFound("Пользователь не найден.");
            }

            var overdueBooks = await _bookRepository.GetOverdueBooksForUserAsync(user.Id);
            if (!overdueBooks.Any())
            {
                return NotFound("Нет просроченных книг для пользователя.");
            }

            return Ok(overdueBooks);
        }

        [HttpGet("genres")]
        public async Task<ActionResult<IEnumerable<object>>> GetGenres()
        {
            var genres = await _genreRepository.GetAllAsync();
            if (!genres.Any())
            {
                return NotFound("Жанры не найдены.");
            }

            return Ok(genres.Select(g => new { g.Id, g.Name }));
        }

        // Получить список просроченных книг
        [HttpGet("overdue")]
        public async Task<ActionResult<IEnumerable<Book>>> GetOverdueBooks()
        {
            var overdueBooks = await _bookRepository.GetOverdueBooksAsync();
            if (!overdueBooks.Any())
            {
                return NotFound("Нет просроченных книг.");
            }

            return Ok(overdueBooks);
        }
    }
}
