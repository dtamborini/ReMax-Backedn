// --- USING NECESSARI ---
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.IdentityModel.Tokens;
using System.Collections.Concurrent;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

var jwtSecretKey = builder.Configuration["JwtSettings:SecretKey"];
var signingKeyId = builder.Configuration["JwtSettings:SigningKeyId"];

// *** VERIFICA CHE LE CHIAVI SIANO CONFIGURATE ***
if (string.IsNullOrEmpty(jwtSecretKey))
{
    throw new InvalidOperationException("JwtSettings:SecretKey non è configurata in appsettings.json o variabili d'ambiente.");
}
if (string.IsNullOrEmpty(signingKeyId))
{
    throw new InvalidOperationException("JwtSettings:SigningKeyId non è configurata in appsettings.json o variabili d'ambiente.");
}

var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins,
                      policy =>
                      {
                          policy.AllowAnyOrigin()
                                .AllowAnyHeader()
                                .AllowAnyMethod();
                      });
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseRouting();

app.UseCors(MyAllowSpecificOrigins);

// *** CONFIGURAZIONE MIDDLEWARE HTTP GENERALE ***
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
// app.UseHttpsRedirection(); // Spesso disabilitato per i mock server locali in Docker per semplicità


// --- Endpoint /oauth/authorize ---
app.MapGet("/oauth/authorize", (HttpRequest request, ILogger<Program> logger) =>
{
    var query = request.Query;
    var responseType = query["response_type"].ToString();
    var clientId = query["client_id"].ToString();
    var redirectUri = query["redirect_uri"].ToString();
    var scope = query["scope"].ToString();
    var state = query["state"].ToString();
    var codeChallenge = query["code_challenge"].ToString();
    var codeChallengeMethod = query["code_challenge_method"].ToString();

    logger.LogInformation("Mock OAuth Authorize Request: ResponseType={ResponseType}, ClientId={ClientId}, RedirectUri={RedirectUri}, Scope={Scope}, State={State}, CodeChallenge={CodeChallenge}, CodeChallengeMethod={CodeChallengeMethod}",
        responseType, clientId, redirectUri, scope, state, codeChallenge, codeChallengeMethod);

    // *** MIGLIORAMENTO DELLE VALIDAZIONI ***
    if (responseType != "code" || string.IsNullOrEmpty(clientId) || string.IsNullOrEmpty(redirectUri))
    {
        logger.LogWarning("Richiesta di autorizzazione non valida: ResponseType={ResponseType}, ClientId={ClientId}, RedirectUri={RedirectUri}",
            responseType, clientId, redirectUri);
        return Results.BadRequest("Invalid authorization request.");
    }

    // *** VALIDAZIONE PKCE INIZIALE ***
    if (!string.IsNullOrEmpty(codeChallenge) && (string.IsNullOrEmpty(codeChallengeMethod) || codeChallengeMethod != "S256"))
    {
        logger.LogWarning("Richiesta PKCE con metodo non supportato o mancante: Method={Method}", codeChallengeMethod);
        return Results.BadRequest("Invalid code_challenge_method. Only S256 is supported.");
    }
    else if (string.IsNullOrEmpty(codeChallenge) && !string.IsNullOrEmpty(codeChallengeMethod))
    {
        logger.LogWarning("Richiesta PKCE con code_challenge_method ma senza code_challenge.");
        return Results.BadRequest("code_challenge_method provided without code_challenge.");
    }

    // Validazione ClientId e RedirectUri specifici per Swagger
    //if (clientId == "swagger-client" && redirectUri != "http://localhost:8081/swagger/oauth2-redirect.html")
    //{
    //    logger.LogWarning("Redirect URI non valido per swagger-client: {RedirectUri}", redirectUri);
    //    return Results.BadRequest(new { error = "invalid_request", error_description = "Invalid redirect_uri for swagger-client." });
    //}
    else if (clientId != "swagger-client" && clientId != "my-client") // Puoi aggiungere altri client ID se ne hai
    {
        logger.LogWarning("Client ID non riconosciuto: {ClientId}", clientId);
        return Results.BadRequest(new { error = "invalid_client", error_description = "Unrecognized client ID." });
    }

    var authCode = $"mock_auth_code_{Guid.NewGuid()}";

    // *** UTILIZZA AuthHelpers.TemporaryAuthorizationCodes ***
    AuthHelpers.TemporaryAuthorizationCodes[authCode] = (clientId, redirectUri, codeChallenge, codeChallengeMethod);

    logger.LogInformation("Authorization code granted: {AuthCode} for client: {ClientId}", authCode, clientId);

    var redirectUrl = QueryHelpers.AddQueryString(redirectUri, "code", authCode);
    if (!string.IsNullOrEmpty(state))
    {
        redirectUrl = QueryHelpers.AddQueryString(redirectUrl, "state", state);
    }

    logger.LogInformation("Reindirizzamento a: {RedirectUrl}", redirectUrl);

    return Results.Redirect(redirectUrl);
})
.WithName("Authorize");


// --- Endpoint /oauth/token ---
app.MapPost("/oauth/token", async (HttpRequest request, ILogger<Program> logger) =>
{
    if (request.ContentType?.Contains("application/x-www-form-urlencoded") != true)
    {
        logger.LogWarning("Richiesta token con Content-Type non valido: {ContentType}", request.ContentType);
        return Results.BadRequest(new { error = "invalid_request", error_description = "Content-Type must be application/x-www-form-urlencoded" });
    }

    var form = await request.ReadFormAsync();
    var grantType = form["grant_type"].ToString();
    var clientId = form["client_id"].ToString();
    var clientSecret = form["client_secret"].ToString();
    var code = form["code"].ToString();
    var redirectUri = form["redirect_uri"].ToString();
    var codeVerifier = form["code_verifier"].ToString();

    var username = form["username"].ToString();
    var password = form["password"].ToString();

    logger.LogInformation("Mock OAuth Token Request: GrantType={GrantType}, ClientId={ClientId}, Code={Code}, CodeVerifierPresent={CodeVerifierPresent}",
        grantType, clientId, code, !string.IsNullOrEmpty(codeVerifier));

    if (grantType == "authorization_code")
    {
        if (string.IsNullOrEmpty(code))
        {
            logger.LogWarning("Codice di autorizzazione mancante nella richiesta token per authorization_code.");
            return Results.BadRequest(new { error = "invalid_grant", error_description = "Authorization code is missing." });
        }

        // *** UTILIZZA AuthHelpers.TemporaryAuthorizationCodes ***
        if (!AuthHelpers.TemporaryAuthorizationCodes.TryRemove(code, out var storedCodeData))
        {
            logger.LogWarning("Codice di autorizzazione non valido o già usato: {Code}", code);
            return Results.BadRequest(new { error = "invalid_grant", error_description = "Invalid or expired authorization code." });
        }

        // *** VERIFICA CLIENTID E REDIRECTURI CONTRO I DATI MEMORIZZATI ***
        if (storedCodeData.clientId != clientId || storedCodeData.redirectUri != redirectUri)
        {
            logger.LogWarning("ClientId o Redirect URI non corrispondenti per il codice: StoredClientId={StoredClientId}, ReceivedClientId={ReceivedClientId}, StoredRedirectUri={StoredRedirectUri}, ReceivedRedirectUri={ReceivedRedirectUri}",
                storedCodeData.clientId, clientId, storedCodeData.redirectUri, redirectUri);
            return Results.BadRequest(new { error = "invalid_request", error_description = "Client ID or Redirect URI mismatch." });
        }

        // --- INIZIO LOGICA PKCE ---
        if (!string.IsNullOrEmpty(storedCodeData.codeChallenge))
        {
            if (string.IsNullOrEmpty(codeVerifier))
            {
                logger.LogWarning("Code Verifier mancante per il flusso PKCE (code_challenge presente nell'authorize).");
                return Results.BadRequest(new { error = "invalid_request", error_description = "Code Verifier is required for PKCE." });
            }

            if (storedCodeData.codeChallengeMethod != "S256")
            {
                logger.LogWarning("Metodo code_challenge non supportato per PKCE: {Method}", storedCodeData.codeChallengeMethod);
                return Results.BadRequest(new { error = "invalid_request", error_description = "Unsupported code_challenge_method for PKCE." });
            }

            // *** UTILIZZA AuthHelpers.ComputeSha256Hash ***
            string calculatedCodeChallenge = AuthHelpers.ComputeSha256Hash(codeVerifier);

            if (calculatedCodeChallenge != storedCodeData.codeChallenge)
            {
                logger.LogWarning("Code Verifier non corrispondente: Calcolato={Calculated}, Atteso={Expected}", calculatedCodeChallenge, storedCodeData.codeChallenge);
                return Results.BadRequest(new { error = "invalid_grant", error_description = "Invalid code verifier." });
            }
            logger.LogInformation("PKCE Code Verifier validato con successo.");
        }
        else if (!string.IsNullOrEmpty(codeVerifier))
        {
            logger.LogWarning("Code Verifier inviato ma PKCE non abilitato/richiesto per questo codice di autorizzazione.");
            return Results.BadRequest(new { error = "invalid_request", error_description = "Code Verifier provided but PKCE was not initiated for this code." });
        }
        // --- FINE LOGICA PKCE ---

        // *** VALIDAZIONE CLIENT SECRET (SE PRESENTE) ***
        if (!string.IsNullOrEmpty(clientSecret) && clientSecret != "swagger-secret")
        {
            logger.LogWarning("Client secret non valido per swagger-client nel flusso Authorization Code.");
            return Results.Unauthorized();
        }

        logger.LogInformation("Scambio codice riuscito per ClientId: {ClientId}", clientId);

        var expiresIn = 3600;

        var tokenHandler = new JwtSecurityTokenHandler();
        var key = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtSecretKey))
        {
            KeyId = signingKeyId
        };

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, "mock_user_id"),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.Name, "testuser"),
                new Claim("scope", "openid profile api1")
            }),
            Expires = DateTime.UtcNow.AddSeconds(expiresIn),
            Issuer = "http://localhost:7005",
            Audience = "api1",
            SigningCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256)
        };

        var jwtToken = tokenHandler.CreateToken(tokenDescriptor);
        var signedJwtString = tokenHandler.WriteToken(jwtToken);

        return Results.Ok(new
        {
            access_token = signedJwtString,
            token_type = "Bearer",
            expires_in = expiresIn,
            scope = "openid profile api1",
            refresh_token = $"mock_refresh_token_for_{clientId}"
        });
    }

    else if (grantType == "password")
    {
        // *** VALIDAZIONE PIÙ ROBUSTA PER IL FLUSSO PASSWORD ***
        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(clientId))
        {
            logger.LogWarning("Parametri mancanti per il flusso password.");
            return Results.BadRequest(new { error = "invalid_request", error_description = "Missing username, password, or client_id for password grant." });
        }

        if (username == "testuser" && password == "testpassword" && clientId == "my-client")
        {
            if (!string.IsNullOrEmpty(clientSecret) && clientSecret != "my-client-secret")
            {
                logger.LogWarning("Client secret non valido per testuser.");
                return Results.Unauthorized();
            }

            logger.LogInformation("Login mock riuscito per utente: {Username}", username);

            var expiresIn = 3600;

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtSecretKey))
            {
                KeyId = signingKeyId
            };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(JwtRegisteredClaimNames.Sub, "mock_user_id_pwd"),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim(ClaimTypes.Name, username),
                    new Claim("scope", "openid profile api1")
                }),
                Expires = DateTime.UtcNow.AddSeconds(expiresIn),
                Issuer = "http://localhost:7005",
                Audience = "api1",
                SigningCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256)
            };

            var jwtToken = tokenHandler.CreateToken(tokenDescriptor);
            var signedJwtString = tokenHandler.WriteToken(jwtToken);

            return Results.Ok(new
            {
                access_token = signedJwtString,
                token_type = "Bearer",
                expires_in = expiresIn,
                scope = "openid profile api1",
                refresh_token = $"mock_refresh_token_for_{username}"
            });
        }
        else
        {
            logger.LogWarning("Credenziali non valide: Username={Username}, ClientId={ClientId}", username, clientId);
            return Results.BadRequest(new
            {
                error = "invalid_grant",
                error_description = "Invalid username, password, or client ID/secret."
            });
        }
    }

    else
    {
        logger.LogWarning("Grant type non supportato: {GrantType}", grantType);
        return Results.BadRequest(new { error = "unsupported_grant_type", error_description = $"Only 'password' and 'authorization_code' grant types are supported. Received: {grantType}" });
    }
})
.WithName("GetOAuthToken");

// *** app.Run() VA SEMPRE ALLA FINE ***
app.Run();

public static class AuthHelpers
{
    // La ConcurrentDictionary va qui dentro
    public static ConcurrentDictionary<string, (string clientId, string redirectUri, string? codeChallenge, string? codeChallengeMethod)> TemporaryAuthorizationCodes = new();

    // Le funzioni helper statiche vanno qui dentro
    public static string ComputeSha256Hash(string rawData)
    {
        using (SHA256 sha256Hash = SHA256.Create())
        {
            byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));
            return Base64UrlEncode(bytes);
        }
    }

    public static string Base64UrlEncode(byte[] input)
    {
        string output = Convert.ToBase64String(input);
        output = output.Replace('+', '-'); // Replace '+' with '-'
        output = output.Replace('/', '_'); // Replace '/' with '_'
        output = output.Replace("=", "");  // Remove padding '='
        return output;
    }
}