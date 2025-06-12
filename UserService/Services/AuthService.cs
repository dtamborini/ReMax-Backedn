using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using UserService.Controllers;
using UserService.Data;
using UserService.Models.Auth;

namespace UserService.Services
{
    public class AuthService : IAuthService
    {
        private readonly IConfiguration _configuration;
        private readonly IPasswordHasher _passwordHasher;
        private readonly UserDbContext _context;
        private readonly ILogger<AuthService> _logger;

        public AuthService(
            IConfiguration configuration,
            IPasswordHasher passwordHasher, 
            UserDbContext context,
            ILogger<AuthService> logger)
        {
            _configuration = configuration;
            _passwordHasher = passwordHasher;
            _context = context;
            _logger = logger;
        }

        public async Task<LoginResult> AuthenticateAsync(string username, string password)
        {
            _logger.LogInformation("Tentativo di login per utente: {Username}", username);

            var user = await _context.Users.SingleOrDefaultAsync(u => u.Username == username);

            if (user == null)
            {
                _logger.LogWarning("Login fallito: Utente '{Username}' non trovato.", username);
                return new LoginResult { IsSuccess = false, ErrorMessage = "Invalid credentials." };
            }

            if (!_passwordHasher.VerifyPassword(password, user.HashPassword))
            {
                _logger.LogWarning("Login fallito: Password non valida per utente '{Username}'.", username);
                return new LoginResult { IsSuccess = false, ErrorMessage = "Invalid credentials." };
            }
            _logger.LogInformation("Login riuscito per utente: {Username}", username);

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration["JwtSettings:Secret"]!);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, user.Username),
                    new Claim(ClaimTypes.NameIdentifier, user.Guid.ToString())
                }),
                Expires = DateTime.UtcNow.AddHours(1), 
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return new LoginResult { IsSuccess = true, Token = tokenHandler.WriteToken(token) };
        }
    }

}