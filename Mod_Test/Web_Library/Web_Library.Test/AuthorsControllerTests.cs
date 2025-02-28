using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Threading.Tasks;
using Web_Library.API.Controllers;
using Web_Library.DTOs;
using Web_Library.Models;
using Web_Library.Services;
using Xunit;

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
        public async Task GetAuthor_ReturnsNotFound_WhenAuthorDoesNotExist()
        {
            _mockAuthorService.Setup(service => service.GetByIdAsync(1)).ReturnsAsync((AuthorDto)null);

            var result = await _controller.GetAuthor(1);
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task CreateAuthor_ReturnsCreatedResult_WhenAuthorIsValid()
        {
            var authorDto = new AuthorDto { FirstName = "John", LastName = "Doe" };
            _mockAuthorService.Setup(service => service.AddAsync(authorDto)).Returns(Task.CompletedTask);
            _mockAuthorService.Setup(service => service.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(authorDto);

            var result = await _controller.CreateAuthor(authorDto);
            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result);
            Assert.Equal(authorDto, createdAtActionResult.Value);
        }

        [Fact]
        public async Task UpdateAuthor_ReturnsNoContent_WhenAuthorIsUpdated()
        {
            var authorDto = new AuthorDto { Id = 1, FirstName = "John", LastName = "Doe" };
            _mockAuthorService.Setup(service => service.UpdateAsync(authorDto)).Returns(Task.CompletedTask);

            var result = await _controller.UpdateAuthor(1, authorDto);
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task DeleteAuthor_ReturnsNoContent_WhenAuthorIsDeleted()
        {
            _mockAuthorService.Setup(service => service.DeleteAsync(1)).Returns(Task.CompletedTask);

            var result = await _controller.DeleteAuthor(1);
            Assert.IsType<NoContentResult>(result);
        }

        public void Dispose()
        {

        }
    }
}