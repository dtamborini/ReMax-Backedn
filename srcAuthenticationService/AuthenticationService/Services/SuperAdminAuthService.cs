using System.Security.Cryptography;
using System.Text;
using AuthenticationService.DTOs;
using AuthenticationService.Interfaces;
using AuthenticationService.Models;
using Microsoft.Extensions.Options;
using Npgsql;
using RemaxManagement.Shared.Auth;
using RemaxManagement.Shared.Models;
using RemaxManagement.Shared.Options;

namespace AuthenticationService.Services
{
    public class SuperAdminAuthService : ISuperAdminAuthService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<SuperAdminAuthService> _logger;
        private readonly IJwtTokenGenerator _jwtTokenGenerator;
        private readonly JwtOptions _jwtOptions;
        private readonly string _connectionString;

        private readonly Dictionary<string, (string Username, DateTime Expiration)> _refreshTokens = new();

        public SuperAdminAuthService(
            IConfiguration configuration,
            ILogger<SuperAdminAuthService> logger,
            IJwtTokenGenerator jwtTokenGenerator,
            IOptions<JwtOptions> jwtOptions)
        {
            _configuration = configuration;
            _logger = logger;
            _jwtTokenGenerator = jwtTokenGenerator;
            _jwtOptions = jwtOptions.Value;
            _connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("DefaultConnection not found in configuration");
        }

        public async Task<(bool Success, SuperAdminLoginResponse? Response, string? Error)> AuthenticateAsync(SuperAdminLoginRequest request)
        {
            try
            {
                _logger.LogInformation("Authenticating super admin: {Username}", request.Username);

                using var connection = new NpgsqlConnection(_connectionString);
                await connection.OpenAsync();

                // Query per trovare il super admin
                const string query = @"
                    SELECT ""Id"", ""Username"", ""Email"", ""PasswordHash"", ""FullName"", ""IsActive"", ""LastLoginAt""
                    FROM ""SuperAdmins""
                    WHERE ""Username"" = @username AND ""IsActive"" = true
                    LIMIT 1";

                using var command = new NpgsqlCommand(query, connection);
                command.Parameters.AddWithValue("@username", request.Username);

                using var reader = await command.ExecuteReaderAsync();
                if (!await reader.ReadAsync())
                {
                    _logger.LogWarning("Super admin not found: {Username}", request.Username);
                    return (false, null, "Invalid username or password");
                }

                var superAdmin = new SuperAdmin
                {
                    Id = reader.GetGuid(reader.GetOrdinal("Id")),
                    Username = reader.GetString(reader.GetOrdinal("Username")),
                    Email = reader.GetString(reader.GetOrdinal("Email")),
                    PasswordHash = reader.GetString(reader.GetOrdinal("PasswordHash")),
                    FullName = reader.IsDBNull(reader.GetOrdinal("FullName")) ? null : reader.GetString(reader.GetOrdinal("FullName")),
                    IsActive = reader.GetBoolean(reader.GetOrdinal("IsActive")),
                    LastLoginAt = reader.IsDBNull(reader.GetOrdinal("LastLoginAt")) ? null : reader.GetDateTime(reader.GetOrdinal("LastLoginAt"))
                };

                // Verifica password
                if (!VerifyPassword(request.Password, superAdmin.PasswordHash))
                {
                    _logger.LogWarning("Invalid password for super admin: {Username}", request.Username);
                    return (false, null, "Invalid username or password");
                }

                // Aggiorna ultimo login
                await UpdateLastLoginAsync(superAdmin.Id);

                // Genera tokens
                var tokens = GenerateTokens(superAdmin);

                var response = new SuperAdminLoginResponse
                {
                    AccessToken = tokens.AccessToken,
                    RefreshToken = tokens.RefreshToken,
                    AdminInfo = new SuperAdminInfo
                    {
                        Id = superAdmin.Id,
                        Username = superAdmin.Username,
                        Email = superAdmin.Email,
                        FullName = superAdmin.FullName,
                        Role = superAdmin.Role,
                        LastLoginAt = DateTime.UtcNow
                    }
                };

                _refreshTokens[tokens.RefreshToken] = (superAdmin.Username, tokens.RefreshTokenExpiration);

                _logger.LogInformation("Super admin authentication successful: {Username}", request.Username);
                return (true, response, null);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during super admin authentication: {Username}", request.Username);
                return (false, null, "An error occurred during authentication");
            }
        }

        public async Task<(bool Success, SuperAdminInfo? AdminInfo, string? Error)> GetMeAsync(string username)
        {
            try
            {
                using var connection = new NpgsqlConnection(_connectionString);
                await connection.OpenAsync();

                const string query = @"
                    SELECT ""Id"", ""Username"", ""Email"", ""FullName"", ""IsActive"", ""LastLoginAt""
                    FROM ""SuperAdmins""
                    WHERE ""Username"" = @username AND ""IsActive"" = true
                    LIMIT 1";

                using var command = new NpgsqlCommand(query, connection);
                command.Parameters.AddWithValue("@username", username);

                using var reader = await command.ExecuteReaderAsync();
                if (!await reader.ReadAsync())
                {
                    return (false, null, "Super admin not found");
                }

                var adminInfo = new SuperAdminInfo
                {
                    Id = reader.GetGuid(reader.GetOrdinal("Id")),
                    Username = reader.GetString(reader.GetOrdinal("Username")),
                    Email = reader.GetString(reader.GetOrdinal("Email")),
                    FullName = reader.IsDBNull(reader.GetOrdinal("FullName")) ? null : reader.GetString(reader.GetOrdinal("FullName")),
                    Role = "SuperAdmin",
                    LastLoginAt = reader.IsDBNull(reader.GetOrdinal("LastLoginAt")) ? null : reader.GetDateTime(reader.GetOrdinal("LastLoginAt"))
                };

                return (true, adminInfo, null);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during GetMe for super admin: {Username}", username);
                return (false, null, "An error occurred while fetching admin information");
            }
        }

        public async Task<bool> ValidateTokenAsync(string token)
        {
            await Task.CompletedTask;
            var principal = _jwtTokenGenerator.ValidateToken(token);
            return principal != null;
        }

        private AuthToken GenerateTokens(SuperAdmin superAdmin)
        {
            var accessTokenExpiration = DateTime.UtcNow.AddMinutes(_jwtOptions.AccessTokenExpiration);
            var refreshTokenExpiration = DateTime.UtcNow.AddMinutes(_jwtOptions.RefreshTokenExpiration);

            var claims = new Dictionary<string, string>
            {
                [System.Security.Claims.ClaimTypes.Name] = superAdmin.Username,
                [System.Security.Claims.ClaimTypes.Role] = superAdmin.Role,
                [JwtConstants.ClaimTypes.Username] = superAdmin.Username,
                [JwtConstants.ClaimTypes.Role] = superAdmin.Role,
                [JwtConstants.ClaimTypes.Email] = superAdmin.Email,
                ["super_admin_id"] = superAdmin.Id.ToString(),
                ["is_super_admin"] = "true"
            };

            if (!string.IsNullOrEmpty(superAdmin.FullName))
                claims[JwtConstants.ClaimTypes.FullName] = superAdmin.FullName;

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

        private async Task UpdateLastLoginAsync(Guid superAdminId)
        {
            try
            {
                using var connection = new NpgsqlConnection(_connectionString);
                await connection.OpenAsync();

                const string updateQuery = @"
                    UPDATE ""SuperAdmins""
                    SET ""LastLoginAt"" = @lastLoginAt
                    WHERE ""Id"" = @id";

                using var command = new NpgsqlCommand(updateQuery, connection);
                command.Parameters.AddWithValue("@lastLoginAt", DateTime.UtcNow);
                command.Parameters.AddWithValue("@id", superAdminId);

                await command.ExecuteNonQueryAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating last login for super admin: {SuperAdminId}", superAdminId);
                // Non interrompere il login per questo errore
            }
        }

        private static bool VerifyPassword(string password, string hash)
        {
            // Per semplicità uso un hash semplice. In produzione usa BCrypt o similar
            using var sha256 = SHA256.Create();
            var passwordBytes = Encoding.UTF8.GetBytes(password);
            var hashBytes = sha256.ComputeHash(passwordBytes);
            var passwordHash = Convert.ToBase64String(hashBytes);
            
            return passwordHash == hash;
        }

        public static string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var passwordBytes = Encoding.UTF8.GetBytes(password);
            var hashBytes = sha256.ComputeHash(passwordBytes);
            return Convert.ToBase64String(hashBytes);
        }
    }
}