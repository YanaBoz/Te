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

        private readonly TimeSpan _timeout = TimeSpan.FromSeconds(10);

        public BooksController(IBookService bookService)
        {
            _bookService = bookService;
        }
        private CancellationToken GetCancellationToken(CancellationToken cancellationToken)
        {
            var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            cts.CancelAfter(_timeout);
            return cts.Token;
        }

        [HttpGet]
        public async Task<IActionResult> GetBooks(CancellationToken cancellationToken, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10, [FromQuery] string? title = null)
        {
            try
            {
                var (books, totalCount) = await _bookService.GetPaginatedBooksAsync(pageNumber, pageSize, GetCancellationToken(cancellationToken), title);
                var bookDtos = books.Adapt<IEnumerable<BookDto>>();
                return Ok(new { TotalCount = totalCount, Books = bookDtos });
            }
            catch (OperationCanceledException)
            {
                throw new TimeoutException("Request Timeout");
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetBook(int id, CancellationToken cancellationToken)
        {
            try
            {
                var book = await _bookService.GetByIdAsync(id, GetCancellationToken(cancellationToken));
                if (book == null) return NotFound();
                return Ok(book.Adapt<BookDto>());
            }
            catch (OperationCanceledException)
            {
                throw new TimeoutException("Request Timeout");
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateBook([FromBody] BookDto bookDto, CancellationToken cancellationToken)
        {
            try
            {
                await _bookService.AddAsync(bookDto, GetCancellationToken(cancellationToken));
                return CreatedAtAction(nameof(GetBook), new { id = bookDto.Id }, bookDto);
            }
            catch (OperationCanceledException)
            {
                throw new TimeoutException("Request Timeout");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateBook(int id, [FromBody] BookDto bookDto, CancellationToken cancellationToken)
        {
            try
            {
                if (id != bookDto.Id) return BadRequest();
                await _bookService.UpdateAsync(bookDto, GetCancellationToken(cancellationToken));
                return NoContent();
            }
            catch (OperationCanceledException)
            {
                throw new TimeoutException("Request Timeout");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBook(int id, CancellationToken cancellationToken)
        {
            try
            {
                await _bookService.DeleteAsync(id, GetCancellationToken(cancellationToken));
                return NoContent();
            }
            catch (OperationCanceledException)
            {
                throw new TimeoutException("Request Timeout");
            }
        }

        [HttpPost("issue/{bookId}")]
        public async Task<IActionResult> IssueBook(int bookId, CancellationToken cancellationToken)
        {
            try
            {
                var username = User.Identity?.Name;
                if (string.IsNullOrEmpty(username))
                    return Unauthorized("User not authenticated.");

                var result = await _bookService.IssueBook(bookId, username, GetCancellationToken(cancellationToken));
                if (!result) return BadRequest("Unable to issue book.");
                return Ok("Book issued successfully!");
            }
            catch (OperationCanceledException)
            {
                throw new TimeoutException("Request Timeout");
            }
        }

        [HttpGet("overdue")]
        public async Task<IActionResult> GetOverdueBooks(CancellationToken cancellationToken)
        {
            try
            {
                var books = await _bookService.GetOverdueBooksAsync(GetCancellationToken(cancellationToken));
                return Ok(books.Adapt<IEnumerable<BookDto>>());
            }
            catch (OperationCanceledException)
            {
                throw new TimeoutException("Request Timeout");
            }
        }

        [HttpGet("user-overdue")]
        public async Task<IActionResult> GetUserOverdueBooks(CancellationToken cancellationToken)
        {
            try
            {
                var username = User.Identity?.Name;
                if (string.IsNullOrEmpty(username))
                    return Unauthorized("User not authenticated.");
                var books = await _bookService.GetOverdueBooksForUserAsync(username, GetCancellationToken(cancellationToken));
                return Ok(books.Adapt<IEnumerable<BookDto>>());
            }
            catch (OperationCanceledException)
            {
                throw new TimeoutException("Request Timeout");
            }
        }
    }
}
