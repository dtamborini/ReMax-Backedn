using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Models;
using System.Security.Claims;
using System.Text;
using System.Text.Json.Serialization;
using WorkSheetService.Clients;
using WorkSheetService.Data;

var builder = WebApplication.CreateBuilder(args);

var useMockOAuth = builder.Configuration.GetValue<bool>("Authentication:UseMockOAuth");
var jwtValidationSecretKey = builder.Configuration["JwtSettings:SecretKey"];
var signingKeyId = builder.Configuration["JwtSettings:SigningKeyId"];

if (string.IsNullOrEmpty(jwtValidationSecretKey) || string.IsNullOrEmpty(signingKeyId))
{
    throw new InvalidOperationException("JWT Secret not found in configuration.");
}

builder.Services.AddHttpClient<IMappingServiceHttpClient, MappingServiceHttpClient>(client => {
    client.BaseAddress = new Uri(builder.Configuration["MappingService:BaseUrl"]!);
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
        Title = "WorkSheet Service API",
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
        tokenUrl = builder.Configuration["OAuthSettings:TokenEndpoint"];
    }

    if (string.IsNullOrEmpty(authorizationUrl) || string.IsNullOrEmpty(tokenUrl))
    {
        throw new InvalidOperationException("OAuth AuthorizationUrl or TokenEndpoint not configured for Swagger.");
    }

    options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.OAuth2,
        Flows = new OpenApiOAuthFlows
        {
            AuthorizationCode = new OpenApiOAuthFlow
            {
                AuthorizationUrl = new Uri(authorizationUrl, UriKind.Absolute),
                TokenUrl = new Uri(tokenUrl, UriKind.Absolute),
                Scopes = swaggerScopes?.ToDictionary(s => s, s => $"Access to {s}") ?? new Dictionary<string, string>(),
                Extensions = new Dictionary<string, IOpenApiExtension>
                {
                    { "x-use-pkce", new OpenApiBoolean(true) }
                }
            }
        },
        Description = "Autenticazione OAuth 2.0 tramite il tuo Identity Provider esterno."
    });

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

builder.Services.AddDbContext<WorkSheetDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));


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
        options.OAuthAppName("WorkSheetService Swagger UI");
        options.OAuthUseBasicAuthenticationWithAccessCodeGrant();
    });
}

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var dbContext = services.GetRequiredService<WorkSheetService.Data.WorkSheetDbContext>();
        dbContext.Database.Migrate();
        Console.WriteLine("Database migration applied successfully.");
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while migrating the database.");
    }
}

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();