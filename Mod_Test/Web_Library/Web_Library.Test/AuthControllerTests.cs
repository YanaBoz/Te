using Moq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using Web_Library.API.Controllers;
using Web_Library.DTOs;
using Web_Library.Models;
using Web_Library.Services;
using Web_Library.Services.Notification;
using Web_Library.Middleware.Exceptions;
using Microsoft.Extensions.Logging;
using Web_Library.Middleware;

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
        public async Task Login_ShouldReturnUnauthorized_WhenCredentialsAreInvalid()
        {
            var loginDto = new LoginDto { Username = "wrongUser", Password = "wrongPassword" };

            _mockUserService.Setup(s => s.LoginAsync(loginDto, It.IsAny<CancellationToken>()))
                .ThrowsAsync(new UnauthorizedException("Invalid username or password"));

            var httpContext = new DefaultHttpContext();
            var middleware = new ExceptionHandlingMiddleware(Mock.Of<ILogger<ExceptionHandlingMiddleware>>());

            var requestDelegate = new RequestDelegate((ctx) => _controller.Login(loginDto, ctx.RequestAborted));
            await middleware.InvokeAsync(httpContext, requestDelegate);

            Assert.Equal(StatusCodes.Status401Unauthorized, httpContext.Response.StatusCode);
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
        public async Task Register_ThrowsBadRequestException_WhenUserAlreadyExists()
        {
            var registerDto = new RegisterDto { Username = "existingUser", Password = "newPassword", FullName = "Existing User" };

            _mockUserService.Setup(s => s.RegisterAsync(It.IsAny<RegisterDto>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new BadRequestException("Username already taken"));

            await Assert.ThrowsAsync<BadRequestException>(() => _controller.Register(registerDto, CancellationToken.None));
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
        public async Task Refresh_ThrowsUnauthorizedException_WhenInvalidToken()
        {
            var refreshTokenDto = new RefreshTokenDto { RefreshToken = "invalidRefreshToken" };

            _mockUserService.Setup(s => s.RefreshTokenAsync(It.IsAny<RefreshTokenDto>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new UnauthorizedException("Invalid refresh token"));

            await Assert.ThrowsAsync<UnauthorizedException>(() => _controller.Refresh(refreshTokenDto, CancellationToken.None));
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
        public async Task GetBorrowedBooks_ThrowsUnauthorizedException_WhenNotAuthenticated()
        {
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            };

            _mockUserService.Setup(s => s.GetBorrowedBooksAsync(null, It.IsAny<CancellationToken>()))
                .ThrowsAsync(new UnauthorizedException("User not authenticated."));

            await Assert.ThrowsAsync<UnauthorizedException>(() => _controller.GetBorrowedBooks(CancellationToken.None));
        }

        [Fact]
        public async Task Login_ReturnsUnauthorized_WhenOperationCanceled()
        {
            var loginDto = new LoginDto { Username = "testUser", Password = "password123" };
            var cts = new CancellationTokenSource();

            _mockUserService
                .Setup(s => s.LoginAsync(It.IsAny<LoginDto>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new OperationCanceledException());

            await Assert.ThrowsAsync<OperationCanceledException>(() => _controller.Login(loginDto, cts.Token));
        }

        [Fact]
        public async Task Login_ReturnsRequestTimeout_WhenOperationCanceledAfterTimeout()
        {
            var loginDto = new LoginDto { Username = "testUser", Password = "password123" };
            using var cts = new CancellationTokenSource(TimeSpan.FromMilliseconds(1000));

            _mockUserService
                .Setup(s => s.LoginAsync(It.IsAny<LoginDto>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new TaskCanceledException());

            await Assert.ThrowsAsync<TaskCanceledException>(() => _controller.Login(loginDto, cts.Token));
        }

        public void Dispose()
        {
            // Dispose resources if necessary
        }
    }
}
