using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace RemaxApi.Shared.Authentication.Middleware
{
    public class JwtValidationMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IConfiguration _configuration;
        private readonly ILogger<JwtValidationMiddleware> _logger;
        private readonly JwtSecurityTokenHandler _tokenHandler;

        public JwtValidationMiddleware(
            RequestDelegate next,
            IConfiguration configuration,
            ILogger<JwtValidationMiddleware> logger)
        {
            _next = next;
            _configuration = configuration;
            _logger = logger;
            _tokenHandler = new JwtSecurityTokenHandler();
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var token = ExtractTokenFromHeader(context.Request);

            if (!string.IsNullOrEmpty(token))
            {
                try
                {
                    var principal = ValidateToken(token);
                    if (principal != null)
                    {
                        context.User = principal;
                        _logger.LogDebug("JWT validation successful for user: {UserId}", 
                            principal.FindFirst("sub")?.Value ?? principal.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                    }
                    else
                    {
                        _logger.LogWarning("JWT validation failed - invalid token");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error during JWT validation");
                }
            }

            await _next(context);
        }

        private string? ExtractTokenFromHeader(HttpRequest request)
        {
            var authHeader = request.Headers["Authorization"].FirstOrDefault();
            if (authHeader?.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase) == true)
            {
                return authHeader.Substring("Bearer ".Length).Trim();
            }
            return authHeader;
        }

        private ClaimsPrincipal? ValidateToken(string token)
        {
            try
            {
                var secretKey = _configuration["ExternalAuth:SecretKey"];

                if (string.IsNullOrEmpty(secretKey))
                {
                    _logger.LogError("ExternalAuth:SecretKey not configured");
                    return null;
                }

                var validationParameters = new TokenValidationParameters
                {
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
                    ClockSkew = TimeSpan.FromMinutes(5)
                };

                var principal = _tokenHandler.ValidateToken(token, validationParameters, out var validatedToken);

                if (validatedToken is JwtSecurityToken jwtToken)
                {
                    _logger.LogDebug("Token validated successfully. Expires: {Expiry}", jwtToken.ValidTo);
                    
                    LogTokenClaims(principal);
                    
                    return principal;
                }

                return null;
            }
            catch (SecurityTokenExpiredException ex)
            {
                _logger.LogWarning("Token expired: {Message}", ex.Message);
                return null;
            }
            catch (SecurityTokenInvalidSignatureException ex)
            {
                _logger.LogWarning("Invalid token signature: {Message}", ex.Message);
                return null;
            }
            catch (SecurityTokenException ex)
            {
                _logger.LogWarning("Token validation failed: {Message}", ex.Message);
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error during token validation");
                return null;
            }
        }

        private void LogTokenClaims(ClaimsPrincipal principal)
        {
            if (_logger.IsEnabled(LogLevel.Debug))
            {
                var claims = principal.Claims.Select(c => $"{c.Type}={c.Value}").ToList();
                _logger.LogDebug("Token claims: {Claims}", string.Join(", ", claims));
            }
        }
    }
}