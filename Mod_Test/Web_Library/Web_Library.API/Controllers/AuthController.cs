using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Web_Library.DTOs;
using Web_Library.Models;
using Web_Library.Services;

namespace Web_Library.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ITokenService _tokenService;
        public AuthController(IUserService userService, ITokenService tokenService)
        {
            _userService = userService;
            _tokenService = tokenService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            var user = await _userService.AuthenticateAsync(loginDto.Username, loginDto.Password);
            if (user == null)
                return Unauthorized("Invalid username or password");

            var accessToken = _tokenService.GenerateAccessToken(user);
            var refreshToken = _tokenService.GenerateRefreshToken(user.Id);
            return Ok(new { AccessToken = accessToken, RefreshToken = refreshToken });
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
        {
            var success = await _userService.RegisterAsync(registerDto);
            if (!success)
                return BadRequest("Registration failed. Username may already be taken.");
            return Ok(new { Message = "User registered successfully" });
        }

        [HttpPost("refresh")]
        public IActionResult Refresh([FromBody] RefreshTokenDto tokenDto)
        {
            var principal = _tokenService.GetPrincipalFromExpiredToken(tokenDto.AccessToken);
            var userId = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null || !_tokenService.ValidateRefreshToken(userId, tokenDto.RefreshToken))
            {
                return Unauthorized("Invalid refresh token");
            }
            var user = new User
            {
                Id = userId,
                Username = principal.Identity?.Name ?? "",
                Role = principal.FindFirst(ClaimTypes.Role)?.Value ?? "User"
            };
            var newAccessToken = _tokenService.GenerateAccessToken(user);
            var newRefreshToken = _tokenService.GenerateRefreshToken(userId);
            return Ok(new { AccessToken = newAccessToken, RefreshToken = newRefreshToken });
        }

        [HttpGet("borrowed-books")]
        public async Task<IActionResult> GetBorrowedBooks()
        {
            var username = User.Identity?.Name;
            if (string.IsNullOrEmpty(username))
                return Unauthorized("User not authenticated.");
            var books = await _userService.GetBorrowedBooksAsync(username);
            return Ok(books);
        }

        [HttpGet("profile")]
        public async Task<IActionResult> GetProfile()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
                return Unauthorized("User not authenticated.");
            var userDto = await _userService.GetUserProfileAsync(userId);
            if (userDto == null)
                return NotFound("User not found.");
            return Ok(userDto);
        }
    }
}
