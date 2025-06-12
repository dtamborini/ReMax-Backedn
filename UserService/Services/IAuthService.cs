using UserService.Models.Auth;

namespace UserService.Services
{
    public interface IAuthService
    {
        Task<LoginResult> AuthenticateAsync(string username, string password);
    }
}