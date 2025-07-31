using BuildingService.Clients;
using BuildingService.Handler;
using BuildingService.Interfaces;
using BuildingService.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Security.Claims;
using System.Text;
using System.Text.Json.Serialization;
using RemaxApi.Shared.Authentication.Extensions;

var builder = WebApplication.CreateBuilder(args);

var useMockOAuth = builder.Configuration.GetValue<bool>("Authentication:UseMockOAuth");
Console.WriteLine($"Using Mock OAuth: {useMockOAuth}");

var jwtValidationSecretKey = builder.Configuration["JwtSettings:SecretKey"];
var signingKeyId = builder.Configuration["JwtSettings:SigningKeyId"];

if (string.IsNullOrEmpty(jwtValidationSecretKey) || string.IsNullOrEmpty(signingKeyId))
{
    throw new InvalidOperationException("JWT Secret not found in configuration.");
}

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<UserClaimService>();
builder.Services.AddScoped<IBuildingFactoryService, BuildingFactoryService>();

// Registra i servizi JWT condivisi
builder.Services.AddExternalJwtAuthentication();

builder.Services.AddHttpClient<IMappingServiceHttpClient, MappingServiceHttpClient>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["MappingService:BaseUrl"]!);
})
.AddHttpMessageHandler(sp =>
{
    var httpContextAccessor = sp.GetRequiredService<IHttpContextAccessor>();
    return new AuthTokenHandler(httpContextAccessor);
});

builder.Services.AddHttpClient<IBuildingDataProviderClient, JsonServerBuildingDataProviderClient>(client => {
    client.BaseAddress = new Uri(builder.Configuration["BuildingDataProviderService:BaseUrl"]!);
});

builder.Services.AddControllers()
    .AddJsonOptions(options => {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Building Service API",
        Version = "v1"
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

    // JWT Bearer Token configuration for Swagger
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header usando Bearer scheme. Esempio: \"Authorization: Bearer {token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT"
    });

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
builder.Services.AddAuthorization(options =>
{
    options.FallbackPolicy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        var useMockOAuth = builder.Configuration.GetValue<bool>("Authentication:UseMockOAuth");

        string? authorizationUrl;
        string? tokenUrl;
        string? swaggerClientId;
        string[]? swaggerScopes;

        if (useMockOAuth)
        {
            swaggerClientId = builder.Configuration["MockOAuthSettings:SwaggerClientId"];
            swaggerScopes = builder.Configuration.GetSection("MockOAuthSettings:SwaggerScopes").Get<string[]>();
            authorizationUrl = builder.Configuration["OAuthSettings:AuthorizationUrl"];
            tokenUrl = builder.Configuration["OAuthSettings:TokenUrl"];
        }
        else
        {
            swaggerClientId = builder.Configuration["OAuthSettings:SwaggerClientId"];
            swaggerScopes = builder.Configuration.GetSection("OAuthSettings:SwaggerScopes").Get<string[]>();
            authorizationUrl = builder.Configuration["OAuthSettings:AuthorizationUrl"];
            tokenUrl = builder.Configuration["OAuthSettings:TokenUrl"];
        }

        options.OAuthClientId(swaggerClientId);
        options.OAuthScopes(swaggerScopes ?? new string[] { });
        options.OAuthUsePkce();
        options.OAuthAppName("BuildingService Swagger UI");
        options.OAuthUseBasicAuthenticationWithAccessCodeGrant();
    });
}

app.UseExternalJwtValidation();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();