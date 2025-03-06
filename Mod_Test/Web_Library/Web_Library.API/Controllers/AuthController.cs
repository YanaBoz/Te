using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Web_Library.DTOs;
using Web_Library.Services;
using Web_Library.Services.Notification;

namespace Web_Library.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly INotificationService _notificationService;

        private readonly TimeSpan _timeout = TimeSpan.FromSeconds(10);

        public AuthController(IUserService userService, INotificationService notificationService)
        {
            _userService = userService;
            _notificationService = notificationService;
        }

        private CancellationToken GetCancellationToken(CancellationToken cancellationToken)
        {
            var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            cts.CancelAfter(_timeout);
            return cts.Token;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto, CancellationToken cancellationToken)
        {
            try
            {
                var tokenResponse = await _userService.LoginAsync(loginDto, GetCancellationToken(cancellationToken));
                if (tokenResponse == null)
                    return Unauthorized("Invalid username or password");

                return Ok(tokenResponse);
            }
            catch (OperationCanceledException)
            {
                throw new TimeoutException("Request Timeout");
            }
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto registerDto, CancellationToken cancellationToken)
        {
            try
            {
                var success = await _userService.RegisterAsync(registerDto, GetCancellationToken(cancellationToken));
                if (!success)
                    return BadRequest("Registration failed. Username may already be taken.");
                return Ok(new { Message = "User registered successfully" });
            }
            catch (OperationCanceledException)
            {
                throw new TimeoutException("Request Timeout");
            }
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh([FromBody] RefreshTokenDto tokenDto, CancellationToken cancellationToken)
        {
            try
            {
                var tokenResponse = await _userService.RefreshTokenAsync(tokenDto, GetCancellationToken(cancellationToken));
                if (tokenResponse == null)
                    return Unauthorized("Invalid refresh token");

                return Ok(tokenResponse);
            }
            catch (OperationCanceledException)
            {
                throw new TimeoutException("Request Timeout");
            }
        }

        [HttpGet("borrowed-books")]
        public async Task<IActionResult> GetBorrowedBooks(CancellationToken cancellationToken)
        {
            try
            {
                var username = User.Identity?.Name;
                if (string.IsNullOrEmpty(username))
                    return Unauthorized("User not authenticated.");

                var books = await _userService.GetBorrowedBooksAsync(username, GetCancellationToken(cancellationToken));
                return Ok(books);
            }
            catch (OperationCanceledException)
            {
                throw new TimeoutException("Request Timeout");
            }
        }

        [HttpGet("has-overdue-books")]
        public async Task<IActionResult> HasOverdueBooks(CancellationToken cancellationToken)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                    return Unauthorized("User not authenticated.");

                bool hasOverdue = await _notificationService.UserHasOverdueBooksAsync(userId, GetCancellationToken(cancellationToken));
                return Ok(new { Message = hasOverdue ? "You have overdue books!" : "No overdue books found." });
            }
            catch (OperationCanceledException)
            {
                throw new TimeoutException("Request Timeout");
            }
        }

        [HttpGet("profile")]
        public async Task<IActionResult> GetProfile(CancellationToken cancellationToken)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (userId == null)
                    return Unauthorized("User not authenticated.");

                var userDto = await _userService.GetUserProfileAsync(userId, GetCancellationToken(cancellationToken));
                if (userDto == null)
                    return NotFound("User not found.");

                return Ok(userDto);
            }
            catch (OperationCanceledException)
            {
                throw new TimeoutException("Request Timeout");
            }
        }
    }
}

