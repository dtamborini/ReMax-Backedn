namespace BuildingService.Handlers
{
    public class LoggingHandler : DelegatingHandler
    {
        private readonly ILogger<LoggingHandler> _logger;

        public LoggingHandler(ILogger<LoggingHandler> logger)
        {
            _logger = logger;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if (request.Method == HttpMethod.Get)
            {
                _logger.LogInformation("Outgoing GET Request: Uri={RequestUri}", request.RequestUri);
                _logger.LogInformation("Request Headers (GET):");
                foreach (var header in request.Headers)
                {
                    _logger.LogInformation("  {HeaderName}: {HeaderValue}", header.Key, string.Join(", ", header.Value));
                }

                if (request.Content != null)
                {
                    _logger.LogInformation("Request Content Headers (GET):");
                    foreach (var header in request.Content.Headers)
                    {
                        _logger.LogInformation("  {HeaderName}: {HeaderValue}", header.Key, string.Join(", ", header.Value));
                    }
                }
            }

            var response = await base.SendAsync(request, cancellationToken);

            if (request.Method == HttpMethod.Get)
            {
                _logger.LogInformation("Incoming Response for GET Request: StatusCode={StatusCode}, Uri={RequestUri}", response.StatusCode, request.RequestUri);
                _logger.LogInformation("Response Headers (GET):");
                foreach (var header in response.Headers)
                {
                    _logger.LogInformation("  {HeaderName}: {HeaderValue}", header.Key, string.Join(", ", header.Value));
                }
                if (response.Content != null)
                {
                    _logger.LogInformation("Response Content (GET):");
                    var responseContent = await response.Content.ReadAsStringAsync();
                    _logger.LogInformation("  Body: {ResponseBody}", responseContent);
                }
            }

            return response;
        }
    }
}