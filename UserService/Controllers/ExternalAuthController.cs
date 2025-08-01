using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserService.Models.Auth;
using UserService.Services;
using System.Security.Claims;

namespace UserService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExternalAuthController : ControllerBase
    {
        private readonly IExternalAuthService _externalAuthService;
        private readonly IMockUserService _mockUserService;
        private readonly ILogger<ExternalAuthController> _logger;

        public ExternalAuthController(
            IExternalAuthService externalAuthService,
            IMockUserService mockUserService,
            ILogger<ExternalAuthController> logger)
        {
            _externalAuthService = externalAuthService;
            _mockUserService = mockUserService;
            _logger = logger;
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var loginResult = await _externalAuthService.AuthenticateAsync(
                model.Username, 
                model.Password, 
                model.ClientId, 
                model.ClientSecret);

            if (loginResult.IsSuccess)
            {
                return Ok(new 
                { 
                    access_token = loginResult.Token,
                    token_type = "Bearer",
                    expires_in = 86400 // 24 ore in secondi
                });
            }
            else
            {
                return Unauthorized(new { 
                    error = "invalid_grant",
                    error_description = loginResult.ErrorMessage 
                });
            }
        }

        [HttpGet("userinfo")]
        [Authorize]
        public async Task<IActionResult> GetUserInfo()
        {
            try
            {
                var userId = User.FindFirst("sub")?.Value ?? User.FindFirst("user_id")?.Value;
                var username = User.FindFirst("username")?.Value ?? User.FindFirst("name")?.Value;
                var email = User.FindFirst("email")?.Value;
                var roles = User.FindAll("role").Select(c => c.Value).ToList();

                if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(username))
                {
                    return Unauthorized(new { error = "invalid_token", error_description = "Token does not contain required user information" });
                }

                var mockUser = await _mockUserService.GetUserByIdAsync(userId);
                if (mockUser == null)
                {
                    return NotFound(new { error = "user_not_found", error_description = "User not found in the system" });
                }

                return Ok(new
                {
                    username = username,
                    role = roles.FirstOrDefault() ?? mockUser.Role,
                    first_name = mockUser.FirstName,
                    last_name = mockUser.LastName,
                    email = email ?? mockUser.Email,
                    user_id = userId
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving user info");
                return StatusCode(500, new { error = "server_error", error_description = "An error occurred while retrieving user information" });
            }
        }
    }
}