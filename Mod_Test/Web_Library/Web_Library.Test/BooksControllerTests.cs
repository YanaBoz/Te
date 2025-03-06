using Moq;
using Web_Library.API.Controllers;
using Web_Library.DTOs;
using Web_Library.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace Web_Library.Tests
{
    public class BooksControllerTests
    {
        private readonly Mock<IBookService> _mockBookService;
        private readonly BooksController _controller;

        public BooksControllerTests()
        {
            _mockBookService = new Mock<IBookService>();
            _controller = new BooksController(_mockBookService.Object);
        }

        [Fact]
        public async Task GetBook_ReturnsNotFound_WhenBookNotExists()
        {
            _mockBookService.Setup(service => service.GetByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((BookDto)null);

            var result = await _controller.GetBook(1, CancellationToken.None);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task GetBook_ReturnsOkResult_WithBookDto()
        {
            var book = new BookDto { Id = 1, Title = "Test Book" };
            _mockBookService.Setup(service => service.GetByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(book);

            var result = await _controller.GetBook(1, CancellationToken.None);

            var actionResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<BookDto>(actionResult.Value);
            Assert.Equal("Test Book", returnValue.Title);
        }

        [Fact]
        public async Task CreateBook_ReturnsCreatedAtAction_WhenSuccessful()
        {
            var bookDto = new BookDto { Id = 1, Title = "New Book" };
            _mockBookService.Setup(service => service.AddAsync(It.IsAny<BookDto>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            var result = await _controller.CreateBook(bookDto, CancellationToken.None);

            var actionResult = Assert.IsType<CreatedAtActionResult>(result);
            Assert.Equal("GetBook", actionResult.ActionName);
            Assert.Equal(1, actionResult.RouteValues["id"]);
        }

        [Fact]
        public async Task UpdateBook_ReturnsNoContent_WhenSuccessful()
        {
            var bookDto = new BookDto { Id = 1, Title = "Updated Book" };
            _mockBookService.Setup(service => service.UpdateAsync(It.IsAny<BookDto>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            var result = await _controller.UpdateBook(1, bookDto, CancellationToken.None);

            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task DeleteBook_ReturnsNoContent_WhenSuccessful()
        {
            _mockBookService.Setup(service => service.DeleteAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            var result = await _controller.DeleteBook(1, CancellationToken.None);

            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task IssueBook_ReturnsOk_WhenSuccessful()
        {
            var username = "testUser";
            var bookId = 1;
            _mockBookService.Setup(service => service.IssueBook(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext() { User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] { new Claim(ClaimTypes.Name, username) })) }
            };

            var result = await _controller.IssueBook(bookId, CancellationToken.None);

            var actionResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("Book issued successfully!", actionResult.Value);
        }

        [Fact]
        public async Task IssueBook_ReturnsUnauthorized_WhenUserNotAuthenticated()
        {
            var bookId = 1;
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            };

            var result = await _controller.IssueBook(bookId, CancellationToken.None);

            var actionResult = Assert.IsType<UnauthorizedObjectResult>(result);
            Assert.Equal("User not authenticated.", actionResult.Value);
        }

        public void Dispose()
        {
            // Dispose resources if necessary
        }
    }
}
