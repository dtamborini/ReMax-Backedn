using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using RemaxManagement.Shared.Options;

namespace RemaxManagement.Shared.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            // Configura JwtOptions
            var jwtSection = configuration.GetSection(JwtOptions.SectionName);
            services.Configure<JwtOptions>(jwtSection);
            
            var jwtOptions = jwtSection.Get<JwtOptions>() ?? throw new InvalidOperationException("JWT configuration is missing");
            
            // Valida la configurazione
            if (string.IsNullOrEmpty(jwtOptions.Key))
                throw new InvalidOperationException("JWT Key is not configured");
            
            // Configura l'autenticazione JWT
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.Key)),
                    ClockSkew = TimeSpan.Zero // Rimuove il default di 5 minuti di tolleranza
                };
                
                // Eventi per logging
                options.Events = new JwtBearerEvents
                {
                    OnAuthenticationFailed = context =>
                    {
                        if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
                        {
                            context.Response.Headers["Token-Expired"] = "true";
                        }
                        return Task.CompletedTask;
                    },
                    OnTokenValidated = context =>
                    {
                        // Qui puoi aggiungere logica custom dopo la validazione del token
                        return Task.CompletedTask;
                    }
                };
            });
            
            return services;
        }
    }
}