using AuthenticationService.DTOs;

namespace AuthenticationService.Interfaces
{
    public interface ISuperAdminAuthService
    {
        Task<(bool Success, SuperAdminLoginResponse? Response, string? Error)> AuthenticateAsync(SuperAdminLoginRequest request);
        Task<(bool Success, SuperAdminInfo? AdminInfo, string? Error)> GetMeAsync(string username);
        Task<bool> ValidateTokenAsync(string token);
    }
}