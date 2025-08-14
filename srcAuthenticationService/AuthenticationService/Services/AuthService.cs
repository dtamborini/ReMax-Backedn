using System.Security.Claims;
using AuthenticationService.DTOs;
using AuthenticationService.Interfaces;
using AuthenticationService.Models;
using RemaxManagement.Shared.Auth;
using Microsoft.Extensions.Options;
using RemaxManagement.Shared.Options;

namespace AuthenticationService.Services
{
    public class AuthService : IAuthService
    {
        private readonly IConfiguration _configuration;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<AuthService> _logger;
        private readonly IJwtTokenGenerator _jwtTokenGenerator;
        private readonly JwtOptions _jwtOptions;
        private readonly bool _useMock;
        
        private readonly List<User> _mockUsers = new()
        {
            new User 
            { 
                Username = "admin", 
                Password = "Admin123!", 
                Role = "Administrator", 
                Email = "admin@condominium.com",
                FullName = "Mario Rossi"
            },
            new User 
            { 
                Username = "supplier", 
                Password = "Supplier123!", 
                Role = "Supplier", 
                Email = "supplier@company.com",
                FullName = "Giuseppe Verdi"
            },
            new User 
            { 
                Username = "resident", 
                Password = "Resident123!", 
                Role = "Resident", 
                Email = "resident@condominium.com",
                FullName = "Anna Bianchi"
            }
        };

        private readonly Dictionary<string, (string Username, DateTime Expiration)> _refreshTokens = new();

        public AuthService(IConfiguration configuration, IHttpClientFactory httpClientFactory, ILogger<AuthService> logger, IJwtTokenGenerator jwtTokenGenerator, IOptions<JwtOptions> jwtOptions)
        {
            _configuration = configuration;
            _httpClientFactory = httpClientFactory;
            _logger = logger;
            _jwtTokenGenerator = jwtTokenGenerator;
            _jwtOptions = jwtOptions.Value;
            _useMock = _configuration.GetValue<bool>("AuthenticationSettings:AuthMock");
        }

        public async Task<(bool Success, LoginResponse? Response, string? Error)> AuthenticateAsync(LoginRequest request)
        {
            try
            {
                if (_useMock)
                {
                    return await AuthenticateWithMockAsync(request);
                }
                else
                {
                    return await AuthenticateWithExternalApiAsync(request);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during authentication");
                return (false, null, "An error occurred during authentication");
            }
        }

        private async Task<(bool Success, LoginResponse? Response, string? Error)> AuthenticateWithMockAsync(LoginRequest request)
        {
            await Task.Delay(100);

            var user = _mockUsers.FirstOrDefault(u => 
                u.Email.Equals(request.Email, StringComparison.OrdinalIgnoreCase) &&
                u.Password == request.Password);

            if (user == null)
            {
                return (false, null, "Invalid username or password");
            }

            var tokens = GenerateTokens(user);
            
            var response = new LoginResponse
            {
                AccessToken = tokens.AccessToken,
                RefreshToken = tokens.RefreshToken
            };

            _refreshTokens[tokens.RefreshToken] = (user.Username, tokens.RefreshTokenExpiration);

            return (true, response, null);
        }

        private async Task<(bool Success, LoginResponse? Response, string? Error)> AuthenticateWithExternalApiAsync(LoginRequest request)
        {
            var httpClient = _httpClientFactory.CreateClient();
            var apiUrl = _configuration["AuthenticationSettings:ExternalAuthApiUrl"];
            
            var response = await httpClient.PostAsJsonAsync($"{apiUrl}/login", request);
            
            if (!response.IsSuccessStatusCode)
            {
                return (false, null, "Authentication failed with external service");
            }

            var externalResponse = await response.Content.ReadFromJsonAsync<LoginResponse>();
            return (true, externalResponse, null);
        }

        public async Task<(bool Success, LoginResponse? Response, string? Error)> RefreshTokenAsync(string refreshToken)
        {
            try
            {
                if (_useMock)
                {
                    return await RefreshTokenWithMockAsync(refreshToken);
                }
                else
                {
                    return await RefreshTokenWithExternalApiAsync(refreshToken);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during token refresh");
                return (false, null, "An error occurred during token refresh");
            }
        }

        private async Task<(bool Success, LoginResponse? Response, string? Error)> RefreshTokenWithMockAsync(string refreshToken)
        {
            await Task.Delay(50);

            if (!_refreshTokens.TryGetValue(refreshToken, out var tokenInfo))
            {
                return (false, null, "Invalid refresh token");
            }

            if (tokenInfo.Expiration < DateTime.UtcNow)
            {
                _refreshTokens.Remove(refreshToken);
                return (false, null, "Refresh token expired");
            }

            var user = _mockUsers.FirstOrDefault(u => u.Username == tokenInfo.Username);
            if (user == null)
            {
                return (false, null, "User not found");
            }

            _refreshTokens.Remove(refreshToken);

            var tokens = GenerateTokens(user);
            
            var response = new LoginResponse
            {
                AccessToken = tokens.AccessToken,
                RefreshToken = tokens.RefreshToken,
            };

            _refreshTokens[tokens.RefreshToken] = (user.Username, tokens.RefreshTokenExpiration);

            return (true, response, null);
        }

        private async Task<(bool Success, LoginResponse? Response, string? Error)> RefreshTokenWithExternalApiAsync(string refreshToken)
        {
            var httpClient = _httpClientFactory.CreateClient();
            var apiUrl = _configuration["AuthenticationSettings:ExternalAuthApiUrl"];
            
            var response = await httpClient.PostAsJsonAsync($"{apiUrl}/refresh", new { refreshToken });
            
            if (!response.IsSuccessStatusCode)
            {
                return (false, null, "Token refresh failed with external service");
            }

            var externalResponse = await response.Content.ReadFromJsonAsync<LoginResponse>();
            return (true, externalResponse, null);
        }

        public async Task<bool> ValidateTokenAsync(string token)
        {
            await Task.CompletedTask;
            var principal = _jwtTokenGenerator.ValidateToken(token);
            return principal != null;
        }

        public async Task<(bool Success, UserInfo? UserInfo, string? Error)> GetMeAsync(string username)
        {
            try
            {
                if (_useMock)
                {
                    return await GetMeWithMockAsync(username);
                }
                else
                {
                    return await GetMeWithExternalApiAsync(username);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during GetMe");
                return (false, null, "An error occurred while fetching user information");
            }
        }

        private async Task<(bool Success, UserInfo? UserInfo, string? Error)> GetMeWithMockAsync(string username)
        {
            await Task.Delay(50);

            var user = _mockUsers.FirstOrDefault(u => 
                u.Username.Equals(username, StringComparison.OrdinalIgnoreCase));

            if (user == null)
            {
                return (false, null, "User not found");
            }

            var userInfo = new UserInfo
            {
                Username = user.Username,
                Role = user.Role,
                Email = user.Email,
                FullName = user.FullName
            };

            return (true, userInfo, null);
        }

        private async Task<(bool Success, UserInfo? UserInfo, string? Error)> GetMeWithExternalApiAsync(string username)
        {
            var httpClient = _httpClientFactory.CreateClient();
            var apiUrl = _configuration["AuthenticationSettings:ExternalAuthApiUrl"];
            
            var response = await httpClient.GetAsync($"{apiUrl}/users/{username}");
            
            if (!response.IsSuccessStatusCode)
            {
                return (false, null, "Failed to fetch user information from external service");
            }

            var userInfo = await response.Content.ReadFromJsonAsync<UserInfo>();
            return (true, userInfo, null);
        }

        private AuthToken GenerateTokens(User user)
        {
            var accessTokenExpiration = DateTime.UtcNow.AddMinutes(_jwtOptions.AccessTokenExpiration);
            var refreshTokenExpiration = DateTime.UtcNow.AddMinutes(_jwtOptions.RefreshTokenExpiration);

            var claims = new Dictionary<string, string>
            {
                [System.Security.Claims.ClaimTypes.Name] = user.Username,
                [System.Security.Claims.ClaimTypes.Role] = user.Role,
                [JwtConstants.ClaimTypes.Username] = user.Username,
                [JwtConstants.ClaimTypes.Role] = user.Role
            };
            
            if (!string.IsNullOrEmpty(user.Email))
                claims[JwtConstants.ClaimTypes.Email] = user.Email;
            
            if (!string.IsNullOrEmpty(user.FullName))
                claims[JwtConstants.ClaimTypes.FullName] = user.FullName;

            var accessToken = _jwtTokenGenerator.GenerateAccessToken(claims);
            var refreshToken = _jwtTokenGenerator.GenerateRefreshToken();

            return new AuthToken
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                AccessTokenExpiration = accessTokenExpiration,
                RefreshTokenExpiration = refreshTokenExpiration
            };
        }

    }
}