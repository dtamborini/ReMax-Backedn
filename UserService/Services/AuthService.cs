using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using UserService.Models.Auth;

namespace UserService.Services
{

    public class AuthService : IAuthService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<AuthService> _logger;
        private readonly HttpClient _httpClient;

        public class OAuthTokenResponse
        {
            [JsonPropertyName("access_token")]
            public string? AccessToken { get; set; }

            [JsonPropertyName("token_type")]
            public string? TokenType { get; set; }

            [JsonPropertyName("expires_in")]
            public int ExpiresIn { get; set; }

            [JsonPropertyName("scope")]
            public string? Scope { get; set; }

            [JsonPropertyName("refresh_token")]
            public string? RefreshToken { get; set; }

            [JsonPropertyName("error")]
            public string? Error { get; set; }

            [JsonPropertyName("error_description")]
            public string? ErrorDescription { get; set; }
        }

        public AuthService(
            IConfiguration configuration,
            ILogger<AuthService> logger,
            HttpClient httpClient)
        {
            _configuration = configuration;
            _logger = logger;
            _httpClient = httpClient;
        }

        public async Task<LoginResult> AuthenticateAsync(string username, string password)
        {
            _logger.LogInformation("Tentativo di autenticazione esterna per utente: {Username}", username);
            try
            {
                bool useMockOAuth = _configuration.GetValue<bool>("Authentication:UseMockOAuth");

                string tokenEndpoint;
                string clientId;
                string? clientSecret;

                if (useMockOAuth)
                {
                    tokenEndpoint = _configuration["MockOAuthSettings:TokenEndpoint"]!;
                    clientId = _configuration["MockOAuthSettings:ClientId"]!;
                    clientSecret = _configuration["MockOAuthSettings:ClientSecret"];
                }
                else
                {
                    tokenEndpoint = _configuration["OAuthSettings:TokenEndpoint"]!;
                    clientId = _configuration["OAuthSettings:ClientId"]!;
                    clientSecret = _configuration["OAuthSettings:ClientSecret"];
                }

                if (string.IsNullOrEmpty(tokenEndpoint) || string.IsNullOrEmpty(clientId))
                {
                    _logger.LogError("Configurazione OAuth mancante (TokenEndpoint o ClientId).");
                    return new LoginResult { IsSuccess = false, ErrorMessage = "Server authentication configuration error." };
                }

                var formContent = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("grant_type", "password"),
                    new KeyValuePair<string, string>("username", username),
                    new KeyValuePair<string, string>("password", password),
                    new KeyValuePair<string, string>("client_id", clientId),
                    new KeyValuePair<string, string>("client_secret", clientSecret ?? "")
                });

                _logger.LogDebug("Invio richiesta token OAuth a: {TokenEndpoint}", tokenEndpoint);

                var response = await _httpClient.PostAsync(tokenEndpoint, formContent);

                if (response.IsSuccessStatusCode)
                {
                    var oauthResponse = await response.Content.ReadFromJsonAsync<OAuthTokenResponse>();

                    if (oauthResponse?.AccessToken != null)
                    {
                        _logger.LogInformation("Token OAuth recuperato con successo da servizio esterno per utente: {Username}", username);
                        return new LoginResult { IsSuccess = true, Token = oauthResponse.AccessToken };
                    }
                    else
                    {
                        _logger.LogError("Risposta OAuth valida ma AccessToken mancante per utente: {Username}. Errore OAuth: {OAuthError}, Descrizione: {ErrorDescription}",
                                         username, oauthResponse?.Error, oauthResponse?.ErrorDescription);
                        return new LoginResult { IsSuccess = false, ErrorMessage = "Failed to retrieve token from external authentication service." };
                    }
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("Errore dal servizio di autenticazione esterno ({StatusCode}) per utente '{Username}': {ErrorContent}",
                                     response.StatusCode, username, errorContent);
                    return new LoginResult { IsSuccess = false, ErrorMessage = $"Authentication failed with external service. Status: {response.StatusCode}" };
                }
            }
            catch (HttpRequestException httpEx)
            {
                _logger.LogError(httpEx, "Errore di rete durante la chiamata al servizio di autenticazione esterno per utente '{Username}'.", username);
                return new LoginResult { IsSuccess = false, ErrorMessage = "Network error during authentication." };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore inatteso durante la chiamata al servizio di autenticazione esterno per utente '{Username}'.", username);
                return new LoginResult { IsSuccess = false, ErrorMessage = "An unexpected error occurred during authentication." };
            }
        }
    }
}