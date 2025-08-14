using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using RemaxManagement.Shared.Options;

namespace RemaxManagement.Shared.Auth
{
    public interface IJwtTokenGenerator
    {
        string GenerateAccessToken(Dictionary<string, string> claims);
        string GenerateRefreshToken();
        ClaimsPrincipal? ValidateToken(string token);
    }
    
    public class JwtTokenGenerator : IJwtTokenGenerator
    {
        private readonly JwtOptions _jwtOptions;
        private readonly JwtSecurityTokenHandler _tokenHandler;
        
        public JwtTokenGenerator(IOptions<JwtOptions> jwtOptions)
        {
            _jwtOptions = jwtOptions.Value;
            _tokenHandler = new JwtSecurityTokenHandler();
        }
        
        public string GenerateAccessToken(Dictionary<string, string> claims)
        {
            var key = Encoding.UTF8.GetBytes(_jwtOptions.Key);
            var tokenClaims = new List<Claim>();
            
            foreach (var claim in claims)
            {
                tokenClaims.Add(new Claim(claim.Key, claim.Value));
            }
            
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(tokenClaims),
                Expires = DateTime.UtcNow.AddMinutes(_jwtOptions.AccessTokenExpiration),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key), 
                    SecurityAlgorithms.HmacSha256Signature)
            };
            
            var token = _tokenHandler.CreateToken(tokenDescriptor);
            return _tokenHandler.WriteToken(token);
        }
        
        public string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }
        
        public ClaimsPrincipal? ValidateToken(string token)
        {
            try
            {
                var key = Encoding.UTF8.GetBytes(_jwtOptions.Key);
                
                var validationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                };
                
                var principal = _tokenHandler.ValidateToken(token, validationParameters, out _);
                return principal;
            }
            catch
            {
                return null;
            }
        }
    }
}