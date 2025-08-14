using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AuthenticationService.DTOs;
using AuthenticationService.Interfaces;

namespace AuthenticationService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IAuthService authService, ILogger<AuthController> logger)
        {
            _authService = authService;
            _logger = logger;
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _logger.LogInformation("Login attempt for user: {Email}", request.Email);

            var result = await _authService.AuthenticateAsync(request);

            if (!result.Success)
            {
                _logger.LogWarning("Login failed for user: {Email}. Error: {Email}", 
                    request.Email, result.Error);
                return Unauthorized(new { error = result.Error });
            }

            _logger.LogInformation("Login successful for user: {Username}", request.Email);
            return Ok(result.Response);
        }

        [HttpPost("refresh")]
        [AllowAnonymous]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _logger.LogInformation("Token refresh attempt");

            var result = await _authService.RefreshTokenAsync(request.RefreshToken);

            if (!result.Success)
            {
                _logger.LogWarning("Token refresh failed. Error: {Error}", result.Error);
                return Unauthorized(new { error = result.Error });
            }

            _logger.LogInformation("Token refresh successful");
            return Ok(result.Response);
        }

        [HttpGet("me")]
        [Authorize]
        public async Task<IActionResult> GetMe()
        {
            var username = User.Identity?.Name;
            
            if (string.IsNullOrEmpty(username))
            {
                return Unauthorized(new { error = "Username not found in token" });
            }

            _logger.LogInformation("GetMe request for user: {Username}", username);

            var result = await _authService.GetMeAsync(username);

            if (!result.Success)
            {
                _logger.LogWarning("GetMe failed for user: {Username}. Error: {Error}", 
                    username, result.Error);
                return NotFound(new { error = result.Error });
            }

            _logger.LogInformation("GetMe successful for user: {Username}", username);
            return Ok(result.UserInfo);
        }

        [HttpPost("logout")]
        [Authorize]
        public IActionResult Logout()
        {
            var username = User.Identity?.Name;
            _logger.LogInformation("User logged out: {Username}", username);
            
            return Ok(new { message = "Logged out successfully" });
        }
    }
}