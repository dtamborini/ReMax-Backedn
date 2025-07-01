using System.Net.Http.Headers;

namespace AssetService.Handler
{
    public class AuthTokenHandler : DelegatingHandler
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AuthTokenHandler(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var httpContext = _httpContextAccessor.HttpContext;
            var incomingRequest = httpContext?.Request;

            var accessToken = incomingRequest?.Headers.Authorization.ToString();

            if (!string.IsNullOrEmpty(accessToken))
            {
                if (accessToken.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
                {
                    accessToken = accessToken.Substring("Bearer ".Length).Trim();
                }

                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            }

            return await base.SendAsync(request, cancellationToken);
        }
    }
}
