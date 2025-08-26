using AuthenticationService.DTOs;
using AuthenticationService.Models;
using RemaxManagement.Shared.MultiTenant;

namespace AuthenticationService.Interfaces
{
    public interface IAuthService
    {
        Task<(bool Success, LoginResponse? Response, string? Error)> AuthenticateAsync(LoginRequest request, Tenant? tenant = null);
        Task<(bool Success, LoginResponse? Response, string? Error)> RefreshTokenAsync(string refreshToken);
        Task<bool> ValidateTokenAsync(string token);
        Task<(bool Success, UserInfo? UserInfo, string? Error)> GetMeAsync(string username);
    }
}