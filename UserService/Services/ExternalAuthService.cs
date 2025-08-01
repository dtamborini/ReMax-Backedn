using UserService.Models.Auth;

namespace UserService.Services
{
    public interface IExternalAuthService
    {
        Task<LoginResult> AuthenticateAsync(string username, string password);
        Task<LoginResult> AuthenticateAsync(string username, string password, string clientId, string? clientSecret);
    }

    public class ExternalAuthService : IExternalAuthService
    {
        private readonly IMockUserService _mockUserService;
        private readonly IJwtTokenService _jwtTokenService;
        private readonly ILogger<ExternalAuthService> _logger;

        public ExternalAuthService(
            IMockUserService mockUserService,
            IJwtTokenService jwtTokenService,
            ILogger<ExternalAuthService> logger)
        {
            _mockUserService = mockUserService;
            _jwtTokenService = jwtTokenService;
            _logger = logger;
        }

        public async Task<LoginResult> AuthenticateAsync(string username, string password)
        {
            return await AuthenticateAsync(username, password, "default-client", null);
        }

        public async Task<LoginResult> AuthenticateAsync(string username, string password, string clientId, string? clientSecret)
        {
            try
            {
                _logger.LogInformation("Authentication attempt for user: {Username} with client: {ClientId}", username, clientId);

                // Valida l'utente
                var user = await _mockUserService.ValidateUserAsync(username, password);
                if (user == null)
                {
                    _logger.LogWarning("Authentication failed - invalid credentials for user: {Username}", username);
                    return new LoginResult
                    {
                        IsSuccess = false,
                        ErrorMessage = "Invalid username or password"
                    };
                }

                // Genera il JWT token
                var token = _jwtTokenService.GenerateToken(
                    userId: user.Id,
                    username: user.Username,
                    email: user.Email,
                    roles: new List<string> { user.Role }
                );

                _logger.LogInformation("Authentication successful for user: {Username} (ID: {UserId})", username, user.Id);

                return new LoginResult
                {
                    IsSuccess = true,
                    Token = token
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error during authentication for user: {Username}", username);
                return new LoginResult
                {
                    IsSuccess = false,
                    ErrorMessage = "An unexpected error occurred during authentication"
                };
            }
        }
    }
}