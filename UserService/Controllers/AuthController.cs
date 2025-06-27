using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using System.Threading.Tasks;
using UserService.Models.Auth;
using UserService.Services;

namespace UserService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {

        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }


        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var loginResult = await _authService.AuthenticateAsync(model.Username, model.Password);

            if (loginResult.IsSuccess)
            {
                return Ok(new { Token = loginResult.Token });
            }
            else
            {
                return Unauthorized(new { Message = loginResult.ErrorMessage });
            }
        }

        [HttpPost("logout")]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await Task.CompletedTask;
            return Ok(new { Message = "Logout successful. Token should be discarded by client." });
        }
    }
}