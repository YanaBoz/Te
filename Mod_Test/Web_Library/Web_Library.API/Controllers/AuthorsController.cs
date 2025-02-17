using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Web_Library.API.Models;
using Web_Library.API.Repositories.AuthorRepo;
using Web_Library.API.Repositories.BookRepo;

namespace Web_Library.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthorsController : ControllerBase
    {
        private readonly IAuthorRepository _authorRepository;
        private readonly IBookRepository _bookRepository;
        public AuthorsController(IAuthorRepository authorRepository, IBookRepository bookRepository)
        {
            _bookRepository = bookRepository;
            _authorRepository = authorRepository;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Author>>> GetAuthors([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var (authors, totalCount) = await _authorRepository.GetPaginatedAuthorsAsync(pageNumber, pageSize);
            return Ok(new { TotalCount = totalCount, Authors = authors });
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Author>> GetAuthor(int id)
        {
            var author = await _authorRepository.GetByIdAsync(id);
            if (author == null) return NotFound();
            return Ok(author);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<Author>> CreateAuthor([FromBody] Author author)
        {
            await _authorRepository.AddAsync(author);
            return CreatedAtAction(nameof(GetAuthor), new { id = author.Id }, author);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateAuthor(int id, [FromBody] Author author)
        {
            if (id != author.Id) return BadRequest();
            await _authorRepository.UpdateAsync(author);
            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteAuthor(int id)
        {
            var author = await _authorRepository.GetByIdAsync(id);
            if (author == null) return NotFound();

            await _bookRepository.DeleteBooksByAuthorIdAsync(id);

            await _authorRepository.DeleteAsync(id);
            return NoContent();
        }
    }
}
