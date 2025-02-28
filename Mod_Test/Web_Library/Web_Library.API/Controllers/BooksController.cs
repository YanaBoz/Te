using Mapster;
using Microsoft.AspNetCore.Mvc;
using Web_Library.DTOs;
using Web_Library.Services;

namespace Web_Library.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BooksController : ControllerBase
    {
        private readonly IBookService _bookService;

        public BooksController(IBookService bookService)
        {
            _bookService = bookService;
        }

        [HttpGet]
        public async Task<IActionResult> GetBooks([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10, [FromQuery] string? title = null)
        {
            var (books, totalCount) = await _bookService.GetPaginatedBooksAsync(pageNumber, pageSize, title);
            var bookDtos = books.Adapt<IEnumerable<BookDto>>();
            return Ok(new { TotalCount = totalCount, Books = bookDtos });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetBook(int id)
        {
            var book = await _bookService.GetByIdAsync(id);
            if (book == null) return NotFound();
            return Ok(book.Adapt<BookDto>());
        }

        [HttpPost]
        public async Task<IActionResult> CreateBook([FromBody] BookDto bookDto)
        {
            await _bookService.AddAsync(bookDto);
            return CreatedAtAction(nameof(GetBook), new { id = bookDto.Id }, bookDto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateBook(int id, [FromBody] BookDto bookDto)
        {
            if (id != bookDto.Id) return BadRequest();
            await _bookService.UpdateAsync(bookDto);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBook(int id)
        {
            await _bookService.DeleteAsync(id);
            return NoContent();
        }

        [HttpPost("issue/{bookId}")]
        public async Task<IActionResult> IssueBook(int bookId)
        {
            var username = User.Identity?.Name;
            if (string.IsNullOrEmpty(username))
                return Unauthorized("User not authenticated.");

            var result = await _bookService.IssueBook(bookId, username);
            if (!result) return BadRequest("Unable to issue book.");
            return Ok("Book issued successfully!");
        }

        [HttpGet("overdue")]
        public async Task<IActionResult> GetOverdueBooks()
        {
            var books = await _bookService.GetOverdueBooksAsync();
            return Ok(books.Adapt<IEnumerable<BookDto>>());
        }

        [HttpGet("user-overdue")]
        public async Task<IActionResult> GetUserOverdueBooks()
        {
            var username = User.Identity?.Name;
            if (string.IsNullOrEmpty(username))
                return Unauthorized("User not authenticated.");
            var books = await _bookService.GetOverdueBooksForUserAsync(username);
            return Ok(books.Adapt<IEnumerable<BookDto>>());
        }
    }
}
