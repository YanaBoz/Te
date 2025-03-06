using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Web_Library.DTOs;
using Web_Library.Services;

namespace Web_Library.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthorsController : ControllerBase
    {
        private readonly IAuthorService _authorService;

        private readonly TimeSpan _timeout = TimeSpan.FromSeconds(10);

        public AuthorsController(IAuthorService authorService)
        {
            _authorService = authorService;
        }
        private CancellationToken GetCancellationToken(CancellationToken cancellationToken)
        {
            var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            cts.CancelAfter(_timeout);
            return cts.Token;
        }

        [HttpGet]
        public async Task<IActionResult> GetAuthors(CancellationToken cancellationToken)
        {
            try
            {
                var authors = await _authorService.GetAllAsync(GetCancellationToken(cancellationToken));
                var authorDtos = authors.Adapt<IEnumerable<AuthorDto>>();
                return Ok(authorDtos);
            }
            catch (OperationCanceledException)
            {
                throw new TimeoutException("Request Timeout");
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetAuthor(int id, CancellationToken cancellationToken)
        {
            try
            {
                var author = await _authorService.GetByIdAsync(id, GetCancellationToken(cancellationToken));
                if (author == null) return NotFound();
                return Ok(author.Adapt<AuthorDto>());
            }
            catch (OperationCanceledException)
            {
                throw new TimeoutException("Request Timeout");
            }
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateAuthor([FromBody] AuthorDto authorDto, CancellationToken cancellationToken)
        {
            try
            {
                await _authorService.AddAsync(authorDto, GetCancellationToken(cancellationToken));

                return CreatedAtAction(nameof(GetAuthor), new { id = authorDto.Id }, authorDto);
            }
            catch (OperationCanceledException)
            {
                throw new TimeoutException("Request Timeout");
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateAuthor(int id, [FromBody] AuthorDto authorDto, CancellationToken cancellationToken)
        {
            try
            {
                if (id != authorDto.Id) return BadRequest();

                await _authorService.UpdateAsync(authorDto, GetCancellationToken(cancellationToken));

                return NoContent();
            }
            catch (OperationCanceledException)
            {
                throw new TimeoutException("Request Timeout");
            }
        }


        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteAuthor(int id, CancellationToken cancellationToken)
        {
            try
            {
                await _authorService.DeleteAsync(id, GetCancellationToken(cancellationToken));
                return NoContent();
            }
            catch (OperationCanceledException)
            {
                throw new TimeoutException("Request Timeout");
            }
        }
    }
}
