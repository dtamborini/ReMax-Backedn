using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace UserService.Services
{
    public interface IJwtTokenService
    {
        string GenerateToken(string userId, string username, string email, List<string> roles);
        string GenerateToken(Dictionary<string, object> claims);
    }

    public class JwtTokenService : IJwtTokenService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<JwtTokenService> _logger;

        public JwtTokenService(IConfiguration configuration, ILogger<JwtTokenService> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public string GenerateToken(string userId, string username, string email, List<string> roles)
        {
            var claims = new Dictionary<string, object>
            {
                { "sub", userId },
                { "user_id", userId },
                { "name", username },
                { "username", username },
                { "email", email },
                { "role", roles }
            };

            return GenerateToken(claims);
        }

        public string GenerateToken(Dictionary<string, object> claims)
        {
            try
            {
                var secretKey = _configuration["ExternalAuth:SecretKey"];
                var issuer = _configuration["ExternalAuth:Issuer"];
                var audience = _configuration["ExternalAuth:Audience"];
                var expirationHours = _configuration.GetValue<int>("ExternalAuth:ExpirationHours", 24);

                if (string.IsNullOrEmpty(secretKey))
                {
                    throw new InvalidOperationException("ExternalAuth:SecretKey not configured");
                }

                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
                var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                var tokenClaims = new List<Claim>();

                foreach (var claim in claims)
                {
                    if (claim.Value is string stringValue)
                    {
                        tokenClaims.Add(new Claim(claim.Key, stringValue));
                    }
                    else if (claim.Value is List<string> listValue)
                    {
                        foreach (var item in listValue)
                        {
                            tokenClaims.Add(new Claim(claim.Key, item));
                        }
                    }
                    else if (claim.Value != null)
                    {
                        tokenClaims.Add(new Claim(claim.Key, claim.Value.ToString()!));
                    }
                }

                tokenClaims.Add(new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()));
                tokenClaims.Add(new Claim(JwtRegisteredClaimNames.Iat, 
                    new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds().ToString(), 
                    ClaimValueTypes.Integer64));

                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(tokenClaims),
                    Expires = DateTime.UtcNow.AddHours(expirationHours),
                    Issuer = issuer,
                    Audience = audience,
                    SigningCredentials = credentials
                };

                var tokenHandler = new JwtSecurityTokenHandler();
                var token = tokenHandler.CreateToken(tokenDescriptor);
                var tokenString = tokenHandler.WriteToken(token);

                _logger.LogDebug("JWT token generated successfully for user: {UserId}", 
                    claims.ContainsKey("sub") ? claims["sub"] : "unknown");

                return tokenString;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating JWT token");
                throw;
            }
        }
    }
}