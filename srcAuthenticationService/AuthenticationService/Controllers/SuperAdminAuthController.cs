using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AuthenticationService.DTOs;
using AuthenticationService.Interfaces;

namespace AuthenticationService.Controllers
{
    [ApiController]
    [Route("api/auth/super-admin")]
    public class SuperAdminAuthController : ControllerBase
    {
        private readonly ISuperAdminAuthService _superAdminAuthService;
        private readonly ILogger<SuperAdminAuthController> _logger;

        public SuperAdminAuthController(
            ISuperAdminAuthService superAdminAuthService, 
            ILogger<SuperAdminAuthController> logger)
        {
            _superAdminAuthService = superAdminAuthService;
            _logger = logger;
        }

        /// <summary>
        /// Login per Super Amministratore - bypassa il sistema multi-tenant
        /// </summary>
        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] SuperAdminLoginRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _logger.LogInformation("Super admin login attempt for username: {Username}", request.Username);

            var result = await _superAdminAuthService.AuthenticateAsync(request);

            if (!result.Success)
            {
                _logger.LogWarning("Super admin login failed for username: {Username}. Error: {Error}", 
                    request.Username, result.Error);
                return Unauthorized(new { error = result.Error });
            }

            _logger.LogInformation("Super admin login successful for username: {Username}", request.Username);
            return Ok(result.Response);
        }

        /// <summary>
        /// Ottiene informazioni del super admin corrente
        /// </summary>
        [HttpGet("me")]
        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> GetMe()
        {
            var username = User.Identity?.Name;
            
            if (string.IsNullOrEmpty(username))
            {
                return Unauthorized(new { error = "Username not found in token" });
            }

            _logger.LogInformation("GetMe request for super admin: {Username}", username);

            var result = await _superAdminAuthService.GetMeAsync(username);

            if (!result.Success)
            {
                _logger.LogWarning("GetMe failed for super admin: {Username}. Error: {Error}", 
                    username, result.Error);
                return NotFound(new { error = result.Error });
            }

            _logger.LogInformation("GetMe successful for super admin: {Username}", username);
            return Ok(result.AdminInfo);
        }

        /// <summary>
        /// Logout per Super Amministratore
        /// </summary>
        [HttpPost("logout")]
        [Authorize(Roles = "SuperAdmin")]
        public IActionResult Logout()
        {
            var username = User.Identity?.Name;
            _logger.LogInformation("Super admin logged out: {Username}", username);
            
            return Ok(new { message = "Super admin logged out successfully" });
        }
    }
}