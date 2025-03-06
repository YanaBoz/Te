using Moq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using Web_Library.API.Controllers;
using Web_Library.DTOs;
using Web_Library.Models;
using Web_Library.Services;
using Web_Library.Services.Notification;

namespace Web_Library.Tests
{
    public class AuthControllerTests
    {
        private readonly Mock<IUserService> _mockUserService;
        private readonly Mock<INotificationService> _mockNotificationService;
        private readonly AuthController _controller;

        public AuthControllerTests()
        {
            _mockUserService = new Mock<IUserService>();
            _mockNotificationService = new Mock<INotificationService>();
            _controller = new AuthController(_mockUserService.Object, _mockNotificationService.Object);
        }

        [Fact]
        public async Task Login_ReturnsOkResult_WithToken_WhenSuccessful()
        {
            var loginDto = new LoginDto { Username = "testUser", Password = "password123" };
            var tokenResponse = new RefreshTokenDto { AccessToken = "validAccessToken", RefreshToken = "validRefreshToken" };

            _mockUserService.Setup(s => s.LoginAsync(It.IsAny<LoginDto>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(tokenResponse);

            var result = await _controller.Login(loginDto, CancellationToken.None);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<RefreshTokenDto>(okResult.Value);
            Assert.Equal("validAccessToken", returnValue.AccessToken);
            Assert.Equal("validRefreshToken", returnValue.RefreshToken);
        }

        [Fact]
        public async Task Login_ReturnsUnauthorized_WhenInvalidCredentials()
        {
            var loginDto = new LoginDto { Username = "wrongUser", Password = "wrongPassword" };

            _mockUserService.Setup(s => s.LoginAsync(It.IsAny<LoginDto>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((RefreshTokenDto)null);

            var result = await _controller.Login(loginDto, CancellationToken.None);

            var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result);
            Assert.Equal("Invalid username or password", unauthorizedResult.Value);
        }

        [Fact]
        public async Task Register_ReturnsOk_WhenSuccessful()
        {
            var registerDto = new RegisterDto { Username = "newUser", Password = "newPassword", FullName = "New User" };

            _mockUserService.Setup(s => s.RegisterAsync(It.IsAny<RegisterDto>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            var result = await _controller.Register(registerDto, CancellationToken.None);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = okResult.Value;
            var messageProp = returnValue.GetType().GetProperty("Message");
            Assert.NotNull(messageProp);
            var message = messageProp.GetValue(returnValue) as string;
            Assert.Equal("User registered successfully", message);
        }


        [Fact]
        public async Task Register_ReturnsBadRequest_WhenUserAlreadyExists()
        {
            var registerDto = new RegisterDto { Username = "existingUser", Password = "newPassword", FullName = "Existing User" };

            _mockUserService.Setup(s => s.RegisterAsync(It.IsAny<RegisterDto>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            var result = await _controller.Register(registerDto, CancellationToken.None);

            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Registration failed. Username may already be taken.", badRequestResult.Value);
        }

        [Fact]
        public async Task Refresh_ReturnsOk_WithNewTokens_WhenValid()
        {
            var refreshTokenDto = new RefreshTokenDto { RefreshToken = "validRefreshToken" };
            var newTokens = new RefreshTokenDto { AccessToken = "newAccessToken", RefreshToken = "newRefreshToken" };

            _mockUserService.Setup(s => s.RefreshTokenAsync(It.IsAny<RefreshTokenDto>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(newTokens);

            var result = await _controller.Refresh(refreshTokenDto, CancellationToken.None);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<RefreshTokenDto>(okResult.Value);
            Assert.Equal("newAccessToken", returnValue.AccessToken);
            Assert.Equal("newRefreshToken", returnValue.RefreshToken);
        }

        [Fact]
        public async Task Refresh_ReturnsUnauthorized_WhenInvalidToken()
        {
            var refreshTokenDto = new RefreshTokenDto { RefreshToken = "invalidRefreshToken" };

            _mockUserService.Setup(s => s.RefreshTokenAsync(It.IsAny<RefreshTokenDto>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((RefreshTokenDto)null);

            var result = await _controller.Refresh(refreshTokenDto, CancellationToken.None);

            var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result);
            Assert.Equal("Invalid refresh token", unauthorizedResult.Value);
        }

        [Fact]
        public async Task GetBorrowedBooks_ReturnsOkResult_WithBooks_WhenAuthenticated()
        {
            var username = "testUser";
            var books = new List<Book>
            {
                new Book { Id = 1, Title = "Test Book 1", ISBN = "123456", Quantity = 3 },
                new Book { Id = 2, Title = "Test Book 2", ISBN = "654321", Quantity = 5 }
            };

            _mockUserService.Setup(s => s.GetBorrowedBooksAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(books);

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity(new[]
                    {
                        new Claim(ClaimTypes.Name, username)
                    }))
                }
            };

            var result = await _controller.GetBorrowedBooks(CancellationToken.None);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<List<Book>>(okResult.Value);
            Assert.Equal(2, returnValue.Count);
        }

        [Fact]
        public async Task GetBorrowedBooks_ReturnsUnauthorized_WhenNotAuthenticated()
        {
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext() 
            };

            var result = await _controller.GetBorrowedBooks(CancellationToken.None);

            var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result);
            Assert.Equal("User not authenticated.", unauthorizedResult.Value);
        }
        public void Dispose()
        {
            // Dispose resources if necessary
        }
    }
}
