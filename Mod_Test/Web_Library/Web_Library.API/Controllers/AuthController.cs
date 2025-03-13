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
            var tokenResponse = await _userService.LoginAsync(loginDto, GetCancellationToken(cancellationToken));
            return Ok(tokenResponse);
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto registerDto, CancellationToken cancellationToken)
        {
            await _userService.RegisterAsync(registerDto, GetCancellationToken(cancellationToken));
            return Ok(new { Message = "User registered successfully" });
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh([FromBody] RefreshTokenDto tokenDto, CancellationToken cancellationToken)
        {
            var tokenResponse = await _userService.RefreshTokenAsync(tokenDto, GetCancellationToken(cancellationToken));
            return Ok(tokenResponse);
        }

        [HttpGet("borrowed-books")]
        public async Task<IActionResult> GetBorrowedBooks(CancellationToken cancellationToken)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var books = await _userService.GetBorrowedBooksAsync(userId, GetCancellationToken(cancellationToken));
            return Ok(books);
        }

        [HttpGet("has-overdue-books")]
        public async Task<IActionResult> HasOverdueBooks(CancellationToken cancellationToken)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var hasOverdue = await _notificationService.UserHasOverdueBooksAsync(userId, GetCancellationToken(cancellationToken));
            return Ok(new { Message = hasOverdue ? "You have overdue books!" : "No overdue books." });
        }

        [HttpGet("profile")]
        public async Task<IActionResult> GetProfile(CancellationToken cancellationToken)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userDto = await _userService.GetUserProfileAsync(userId, GetCancellationToken(cancellationToken));
            return Ok(userId);
        }
    }
}
