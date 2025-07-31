using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserService.Models.Auth;
using UserService.Services;

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

        [HttpPost("token")]
        [AllowAnonymous]
        public async Task<IActionResult> Token([FromForm] string grant_type, 
                                             [FromForm] string username, 
                                             [FromForm] string password,
                                             [FromForm] string client_id,
                                             [FromForm] string? client_secret)
        {
            if (grant_type != "password")
            {
                return BadRequest(new { 
                    error = "unsupported_grant_type",
                    error_description = "Only 'password' grant type is supported" 
                });
            }

            var loginResult = await _externalAuthService.AuthenticateAsync(username, password, client_id, client_secret);

            if (loginResult.IsSuccess)
            {
                return Ok(new 
                { 
                    access_token = loginResult.Token,
                    token_type = "Bearer",
                    expires_in = 86400
                });
            }
            else
            {
                return BadRequest(new { 
                    error = "invalid_grant",
                    error_description = loginResult.ErrorMessage 
                });
            }
        }

        [HttpGet("users")]
        [AllowAnonymous]
        public async Task<IActionResult> GetUsers()
        {
            var users = await _mockUserService.GetAllUsersAsync();
            var userList = users.Select(u => new
            {
                id = u.Id,
                username = u.Username,
                email = u.Email,
                firstName = u.FirstName,
                lastName = u.LastName,
                roles = u.Roles,
                isActive = u.IsActive
            }).ToList();

            return Ok(new { users = userList });
        }

        [HttpGet("test-tokens")]
        [AllowAnonymous]
        public async Task<IActionResult> GetTestTokens()
        {
            var users = await _mockUserService.GetAllUsersAsync();
            var tokens = new List<object>();

            foreach (var user in users.Take(3)) // Solo i primi 3 utenti
            {
                var loginResult = await _externalAuthService.AuthenticateAsync(user.Username, user.Password);
                if (loginResult.IsSuccess)
                {
                    tokens.Add(new
                    {
                        username = user.Username,
                        password = user.Password,
                        roles = user.Roles,
                        token = loginResult.Token
                    });
                }
            }

            return Ok(new { 
                message = "Test tokens generated for development/testing",
                tokens = tokens 
            });
        }
    }
}