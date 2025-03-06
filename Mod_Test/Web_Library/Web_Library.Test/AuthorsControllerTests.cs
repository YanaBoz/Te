using Microsoft.AspNetCore.Mvc;
using Moq;
using Web_Library.API.Controllers;
using Web_Library.DTOs;
using Web_Library.Services;

namespace Web_Library.Test
{
    public class AuthorsControllerTests : IDisposable
    {
        private readonly Mock<IAuthorService> _mockAuthorService;
        private readonly AuthorsController _controller;

        public AuthorsControllerTests()
        {
            _mockAuthorService = new Mock<IAuthorService>();
            _controller = new AuthorsController(_mockAuthorService.Object);
        }

        [Fact]
        public async Task GetAuthors_ReturnsOk_WhenAuthorsExist()
        {
            var authors = new List<AuthorDto>
            {
                new AuthorDto { Id = 1, FirstName = "John", LastName = "Doe" },
                new AuthorDto { Id = 2, FirstName = "Jane", LastName = "Doe" }
            };

            _mockAuthorService.Setup(service => service.GetAllAsync(It.IsAny<CancellationToken>())).ReturnsAsync(authors);

            var result = await _controller.GetAuthors(CancellationToken.None);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsAssignableFrom<IEnumerable<AuthorDto>>(okResult.Value);
            Assert.Equal(2, returnValue.Count());
        }

        [Fact]
        public async Task GetAuthors_ThrowsTimeoutException_WhenOperationIsCancelled()
        {
            _mockAuthorService.Setup(service => service.GetAllAsync(It.IsAny<CancellationToken>())).ThrowsAsync(new OperationCanceledException());

            var result = await Assert.ThrowsAsync<TimeoutException>(() => _controller.GetAuthors(CancellationToken.None));
            Assert.Equal("Request Timeout", result.Message);
        }

        [Fact]
        public async Task GetAuthor_ReturnsOk_WhenAuthorExists()
        {
            var authorDto = new AuthorDto { Id = 1, FirstName = "John", LastName = "Doe" };
            _mockAuthorService.Setup(service => service.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(authorDto);

            var result = await _controller.GetAuthor(1, CancellationToken.None);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<AuthorDto>(okResult.Value);
            Assert.Equal(authorDto.Id, returnValue.Id);
        }

        [Fact]
        public async Task GetAuthor_ReturnsNotFound_WhenAuthorDoesNotExist()
        {
            _mockAuthorService.Setup(service => service.GetByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>())).ReturnsAsync((AuthorDto)null);

            var result = await _controller.GetAuthor(999, CancellationToken.None);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task CreateAuthor_ReturnsCreatedAtAction_WhenAuthorIsValid()
        {
            var authorDto = new AuthorDto { FirstName = "John", LastName = "Doe" };
            _mockAuthorService.Setup(service => service.AddAsync(It.IsAny<AuthorDto>(), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

            var result = await _controller.CreateAuthor(authorDto, CancellationToken.None);

            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result);
            Assert.Equal("GetAuthor", createdAtActionResult.ActionName);
            Assert.Equal(authorDto, createdAtActionResult.Value);
        }

        [Fact]
        public async Task UpdateAuthor_ReturnsNoContent_WhenAuthorIsUpdated()
        {
            var authorDto = new AuthorDto { Id = 1, FirstName = "John", LastName = "Doe" };
            _mockAuthorService.Setup(service => service.UpdateAsync(It.IsAny<AuthorDto>(), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

            var result = await _controller.UpdateAuthor(1, authorDto, CancellationToken.None);

            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task DeleteAuthor_ReturnsNoContent_WhenAuthorIsDeleted()
        {
            _mockAuthorService.Setup(service => service.DeleteAsync(It.IsAny<int>(), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

            var result = await _controller.DeleteAuthor(1, CancellationToken.None);

            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task DeleteAuthor_ThrowsException_WhenAuthorDoesNotExist()
        {
            _mockAuthorService
                .Setup(service => service.DeleteAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("Author not found"));

            var exception = await Assert.ThrowsAsync<Exception>(() =>
                _controller.DeleteAuthor(999, CancellationToken.None));

            Assert.Equal("Author not found", exception.Message);
        }


        [Fact]
        public async Task CreateAuthor_ThrowsTimeoutException_WhenOperationIsCancelled()
        {
            var authorDto = new AuthorDto { FirstName = "John", LastName = "Doe" };
            _mockAuthorService.Setup(service => service.AddAsync(It.IsAny<AuthorDto>(), It.IsAny<CancellationToken>())).ThrowsAsync(new OperationCanceledException());

            var result = await Assert.ThrowsAsync<TimeoutException>(() => _controller.CreateAuthor(authorDto, CancellationToken.None));
            Assert.Equal("Request Timeout", result.Message);
        }

        [Fact]
        public async Task UpdateAuthor_ThrowsTimeoutException_WhenOperationIsCancelled()
        {
            var authorDto = new AuthorDto { Id = 1, FirstName = "John", LastName = "Doe" };
            _mockAuthorService.Setup(service => service.UpdateAsync(It.IsAny<AuthorDto>(), It.IsAny<CancellationToken>())).ThrowsAsync(new OperationCanceledException());

            var result = await Assert.ThrowsAsync<TimeoutException>(() => _controller.UpdateAuthor(1, authorDto, CancellationToken.None));
            Assert.Equal("Request Timeout", result.Message);
        }

        [Fact]
        public async Task DeleteAuthor_ThrowsTimeoutException_WhenOperationIsCancelled()
        {
            _mockAuthorService.Setup(service => service.DeleteAsync(It.IsAny<int>(), It.IsAny<CancellationToken>())).ThrowsAsync(new OperationCanceledException());

            var result = await Assert.ThrowsAsync<TimeoutException>(() => _controller.DeleteAuthor(1, CancellationToken.None));
            Assert.Equal("Request Timeout", result.Message);
        }

        public void Dispose()
        {
            // Dispose resources if necessary
        }
    }
}
