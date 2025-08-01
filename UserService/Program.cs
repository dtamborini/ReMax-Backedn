using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Security.Claims;
using System.Text;
using System.Text.Json.Serialization;
using UserService.Clients;
using UserService.Handler;
using UserService.Models.DTOs;
using UserService.Services;

var builder = WebApplication.CreateBuilder(args);

var useMockOAuth = builder.Configuration.GetValue<bool>("Authentication:UseMockOAuth");
var jwtValidationSecretKey = builder.Configuration["JwtSettings:SecretKey"];
var signingKeyId = builder.Configuration["JwtSettings:SigningKeyId"];

if (string.IsNullOrEmpty(jwtValidationSecretKey) || string.IsNullOrEmpty(signingKeyId))
{
    throw new InvalidOperationException("JWT Secret not found in configuration.");
}

builder.Services.AddHttpContextAccessor();
builder.Services.AddHttpClient<IMappingServiceHttpClient, MappingServiceHttpClient>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["MappingService:BaseUrl"]!);
})
.AddHttpMessageHandler(sp =>
{
    var httpContextAccessor = sp.GetRequiredService<IHttpContextAccessor>();
    return new AuthTokenHandler(httpContextAccessor);
});

builder.Services.AddHttpClient<IUserDataProviderClient, UserDataProviderClient>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["UserDataProviderService:BaseUrl"]!);
});


builder.Services.AddHttpClient<IAuthService, AuthService>(client =>
{
    string? baseUrl;
    if (useMockOAuth)
    {
        baseUrl = builder.Configuration["MockOAuthSettings:BaseUrl"];
    }
    else
    {
        baseUrl = builder.Configuration["OAuthSettings:BaseUrl"];
    }

    if (!string.IsNullOrEmpty(baseUrl))
    {
        client.BaseAddress = new Uri(baseUrl);
    }
});

// Servizi per l'autenticazione JWT esterna
builder.Services.AddScoped<IJwtTokenService, JwtTokenService>();
builder.Services.AddScoped<IMockUserService, MockUserService>();
builder.Services.AddScoped<IExternalAuthService, ExternalAuthService>();


builder.Services.AddControllers()
    .AddJsonOptions(options => {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "User Service API",
        Version = "v1",
        Description = "API del servizio utenti con autenticazione OAuth 2.0."
    });

    var useMockOAuth = builder.Configuration.GetValue<bool>("Authentication:UseMockOAuth");


    string? authorizationUrl;
    string? tokenUrl;
    string? swaggerClientId;
    string[]? swaggerScopes;

    if (useMockOAuth)
    {
        swaggerClientId = builder.Configuration["MockOAuthSettings:SwaggerClientId"];
        swaggerScopes = builder.Configuration.GetSection("MockOAuthSettings:SwaggerScopes").Get<string[]>();
        authorizationUrl = builder.Configuration["MockOAuthSettings:AuthorizationUrl"];
        tokenUrl = builder.Configuration["MockOAuthSettings:TokenUrl"];
    }
    else
    {
        swaggerClientId = builder.Configuration["OAuthSettings:SwaggerClientId"];
        swaggerScopes = builder.Configuration.GetSection("OAuthSettings:SwaggerScopes").Get<string[]>();
        authorizationUrl = builder.Configuration["OAuthSettings:AuthorizationUrl"];
        tokenUrl = builder.Configuration["OAuthSettings:TokenUrl"];
    }

    if (string.IsNullOrEmpty(authorizationUrl) || string.IsNullOrEmpty(tokenUrl))
    {
        throw new InvalidOperationException("OAuth AuthorizationUrl or TokenEndpoint not configured for Swagger.");
    }

    // OAuth2 Security Scheme
    options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.OAuth2,
        Flows = new OpenApiOAuthFlows
        {
            AuthorizationCode = new OpenApiOAuthFlow
            {
                AuthorizationUrl = new Uri(authorizationUrl, UriKind.Absolute),
                TokenUrl = new Uri(tokenUrl, UriKind.Absolute),
                Scopes = swaggerScopes?.ToDictionary(s => s, s => $"Access to {s}") ?? new Dictionary<string, string>()
            }
        },
        Description = "Autenticazione OAuth 2.0 tramite il tuo Identity Provider esterno."
    });

    // JWT Bearer Security Scheme
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Name = "Authorization",
        Description = "Inserisci il JWT Bearer token ottenuto da /api/ExternalAuth/login"
    });

    // OAuth2 Security Requirement
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "oauth2"
                }
            },
            swaggerScopes ?? new string[] {}
        }
    });

    // Bearer Token Security Requirement
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});

// --- Configurazione Servizi di Autenticazione ---
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
{
    options.Events = new JwtBearerEvents
    {
        OnTokenValidated = context =>
        {
            Console.WriteLine($"Token Validated for user: {context.Principal?.Identity?.Name}");
            foreach (var claim in context.Principal?.Claims ?? Enumerable.Empty<Claim>())
            {
                Console.WriteLine($"  Claim: {claim.Type} = {claim.Value}");
            }
            return Task.CompletedTask;
        },
        OnAuthenticationFailed = context =>
        {
            Console.WriteLine($"Authentication failed: {context.Exception.Message}");
            return Task.FromException(context.Exception);
        },
        OnChallenge = context =>
        {
            Console.WriteLine($"OnChallenge called. Reason: {context.AuthenticateFailure?.Message ?? "None"}");
            return Task.CompletedTask;
        },
        OnForbidden = context =>
        {
            Console.WriteLine($"OnForbidden called. User: {context.Principal?.Identity?.Name}");
            return Task.CompletedTask;
        }
    };

    if (useMockOAuth)
    {
        options.RequireHttpsMetadata = false;
        options.SaveToken = true;
        
        // Use only ExternalAuth configuration for simplicity
        var externalAuthSecretKey = builder.Configuration["ExternalAuth:SecretKey"];
        
        // Debug logging for production troubleshooting
        Console.WriteLine($"JWT Validation - ExternalAuth:SecretKey present: {!string.IsNullOrEmpty(externalAuthSecretKey)}, Length: {externalAuthSecretKey?.Length ?? 0}");
        
        if (!string.IsNullOrEmpty(externalAuthSecretKey))
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.FromMinutes(5),
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(externalAuthSecretKey))
            };
        }
        else
        {
            // Fallback to original configuration
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = "http://localhost:7005",
                ValidateAudience = true,
                ValidAudience = "api1",
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtValidationSecretKey))
                {
                    KeyId = signingKeyId
                }
            };
        }
    }
    else
    {
        options.Authority = builder.Configuration["OAuthSettings:Authority"];
        options.Audience = builder.Configuration["OAuthSettings:Audience"];
        options.RequireHttpsMetadata = builder.Environment.IsProduction();
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true
        };
    }
});

// --- Configurazione Servizi di Autorizzazione ---
builder.Services.AddAuthorization();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options => 
    {
        var useMockOAuth = builder.Configuration.GetValue<bool>("Authentication:UseMockOAuth");

        string? swaggerClientId;
        string[]? swaggerScopes;

        if (useMockOAuth)
        {
            swaggerClientId = builder.Configuration["MockOAuthSettings:SwaggerClientId"];
            swaggerScopes = builder.Configuration.GetSection("MockOAuthSettings:SwaggerScopes").Get<string[]>();
        }
        else
        {
            swaggerClientId = builder.Configuration["OAuthSettings:SwaggerClientId"];
            swaggerScopes = builder.Configuration.GetSection("OAuthSettings:SwaggerScopes").Get<string[]>();
        }

        options.OAuthClientId(swaggerClientId);
        options.OAuthScopes(swaggerScopes ?? new string[] { });
        options.OAuthUsePkce();
        options.OAuthAppName("User Service Swagger UI");
        options.OAuthUseBasicAuthenticationWithAccessCodeGrant();
    });

}

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();