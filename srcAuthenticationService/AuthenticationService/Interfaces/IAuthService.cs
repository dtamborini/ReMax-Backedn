using AuthenticationService.DTOs;
using AuthenticationService.Models;

namespace AuthenticationService.Interfaces
{
    public interface IAuthService
    {
        Task<(bool Success, LoginResponse? Response, string? Error)> AuthenticateAsync(LoginRequest request);
        Task<(bool Success, LoginResponse? Response, string? Error)> RefreshTokenAsync(string refreshToken);
        Task<bool> ValidateTokenAsync(string token);
        Task<(bool Success, UserInfo? UserInfo, string? Error)> GetMeAsync(string username);
    }
}