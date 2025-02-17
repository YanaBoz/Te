using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;
using Web_Library.API.Models;
using Web_Library.API.Services;

namespace Web_Library.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly ITokenService _tokenService;
        private readonly IUserService _userService;

        public AuthController(ITokenService tokenService, IUserService userService)
        {
            _tokenService = tokenService;
            _userService = userService;
        }

        [HttpPost("token")]
        public async Task<IActionResult> Token([FromBody] LoginModel model)
        {
            var user = await _userService.AuthenticateAsync(model.Username, model.Password);
            if (user == null)
                return Unauthorized(new { message = "Invalid username or password" });

            var accessToken = _tokenService.GenerateAccessToken(user);
            var refreshToken = _tokenService.GenerateRefreshToken(user.Id);

            return Ok(new { AccessToken = accessToken, RefreshToken = refreshToken });
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterModel model)
        {
            var user = new User { Username = model.Username, FullName = model.FullName };
            var result = await _userService.RegisterAsync(user, model.Password);

            if (!result)
                return BadRequest(new { message = "User registration failed. Username may already be taken." });

            return Ok(new { message = "User registered successfully" });
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh([FromBody] RefreshTokenModel model)
        {
            var principal = _tokenService.GetPrincipalFromExpiredToken(model.AccessToken);
            var userIdClaim = principal.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
                return Unauthorized();

            var userId = userIdClaim.Value;
            var user = await _userService.GetUserByIdAsync(userId);
            if (user == null || !_tokenService.ValidateRefreshToken(userId, model.RefreshToken))
                return Unauthorized();

            var newAccessToken = _tokenService.GenerateAccessToken(user);
            var newRefreshToken = _tokenService.GenerateRefreshToken(user.Id);

            return Ok(new { AccessToken = newAccessToken, RefreshToken = newRefreshToken });
        }

        [HttpGet("borrowed-books")]
        public async Task<IActionResult> GetBorrowedBooks()
        {
            var username = User.FindFirstValue(ClaimTypes.NameIdentifier); // Извлечение никнейма
            if (string.IsNullOrEmpty(username))
            {
                return Unauthorized("Пользователь не аутентифицирован.");
            }

            // Найти пользователя по никнейму
            var user = await _userService.GetUserByUsernameAsync(username);
            if (user == null)
            {
                return NotFound("Пользователь не найден.");
            }
            if (string.IsNullOrEmpty(username))
                return Unauthorized();

            var books = await _userService.GetBorrowedBooksAsync(user.Id);

            if (books == null || !books.Any())
            {
                return NotFound("У пользователя нет взятых книг.");
            }

            return Ok(books);
        }
        [HttpGet("profile")]
        public async Task<IActionResult> GetUserProfile()
        {
            var username = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (username == null)
                return Unauthorized("User not authenticated.");

            var user = await _userService.GetUserByUsernameAsync(username);

            if (user == null)
                return NotFound("User not found.");

            // Return user information excluding sensitive data
            return Ok(new
            {
                user.Id,
                user.Username,
                user.FullName,
                user.Role
            });
        }
    }
}
