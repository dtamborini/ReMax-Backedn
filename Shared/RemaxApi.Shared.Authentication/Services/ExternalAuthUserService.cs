using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Security.Claims;

namespace RemaxApi.Shared.Authentication.Services
{
    public interface IExternalAuthUserService
    {
        string? GetUserId();
        string? GetUserName();
        string? GetUserEmail();
        List<string> GetUserRoles();
        Dictionary<string, string> GetAllClaims();
        bool IsAuthenticated();
    }

    public class ExternalAuthUserService : IExternalAuthUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<ExternalAuthUserService> _logger;

        public ExternalAuthUserService(
            IHttpContextAccessor httpContextAccessor,
            ILogger<ExternalAuthUserService> logger)
        {
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
        }

        public string? GetUserId()
        {
            var user = _httpContextAccessor.HttpContext?.User;
            if (user?.Identity?.IsAuthenticated != true) return null;

            return user.FindFirst("sub")?.Value ??
                   user.FindFirst(ClaimTypes.NameIdentifier)?.Value ??
                   user.FindFirst("user_id")?.Value;
        }

        public string? GetUserName()
        {
            var user = _httpContextAccessor.HttpContext?.User;
            if (user?.Identity?.IsAuthenticated != true) return null;

            return user.FindFirst("name")?.Value ??
                   user.FindFirst(ClaimTypes.Name)?.Value ??
                   user.FindFirst("username")?.Value;
        }

        public string? GetUserEmail()
        {
            var user = _httpContextAccessor.HttpContext?.User;
            if (user?.Identity?.IsAuthenticated != true) return null;

            return user.FindFirst("email")?.Value ??
                   user.FindFirst(ClaimTypes.Email)?.Value;
        }

        public List<string> GetUserRoles()
        {
            var user = _httpContextAccessor.HttpContext?.User;
            if (user?.Identity?.IsAuthenticated != true) return new List<string>();

            return user.FindAll("role")
                      .Concat(user.FindAll(ClaimTypes.Role))
                      .Select(c => c.Value)
                      .Distinct()
                      .ToList();
        }

        public Dictionary<string, string> GetAllClaims()
        {
            var user = _httpContextAccessor.HttpContext?.User;
            if (user?.Identity?.IsAuthenticated != true) return new Dictionary<string, string>();

            return user.Claims.ToDictionary(c => c.Type, c => c.Value);
        }

        public bool IsAuthenticated()
        {
            return _httpContextAccessor.HttpContext?.User?.Identity?.IsAuthenticated == true;
        }
    }
}