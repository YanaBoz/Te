using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using Web_Library.API.Controllers;
using Web_Library.API.Models;
using Web_Library.API.Repositories.AuthorRepo;
using Xunit;

namespace Web_Library.Test
{
    public class AuthorsControllerTests : IDisposable
    {
        private readonly Mock<IAuthorRepository> _mockAuthorRepository;
        private readonly AuthorsController _controller;

        public AuthorsControllerTests()
        {
            _mockAuthorRepository = new Mock<IAuthorRepository>();
            _controller = new AuthorsController(_mockAuthorRepository.Object, null);
        }

        [Fact]
        public async Task GetAuthor_ReturnsNotFound_WhenAuthorDoesNotExist()
        {
            _mockAuthorRepository.Setup(repo => repo.GetByIdAsync(1)).ReturnsAsync((Author)null);
            var result = await _controller.GetAuthor(1);
            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task CreateAuthor_ReturnsCreatedResult_WhenAuthorIsValid()
        {
            var author = new Author { FirstName = "John", LastName = "Doe", BirthDate = new DateTime(1980, 1, 1), Country = "USA" };
            _mockAuthorRepository.Setup(repo => repo.AddAsync(author)).Returns(Task.CompletedTask);
            _mockAuthorRepository.Setup(repo => repo.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(author);

            var result = await _controller.CreateAuthor(author);
            var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            Assert.Equal(author, createdResult.Value);
        }

        [Fact]
        public async Task UpdateAuthor_ReturnsNoContent_WhenAuthorIsUpdated()
        {
            var author = new Author { Id = 1, FirstName = "John", LastName = "Doe", BirthDate = new DateTime(1980, 1, 1), Country = "USA" };
            _mockAuthorRepository.Setup(repo => repo.UpdateAsync(author)).Returns(Task.CompletedTask);

            var result = await _controller.UpdateAuthor(1, author);
            Assert.IsType<NoContentResult>(result);
        }

        public void Dispose()
        {
        }
    }
}