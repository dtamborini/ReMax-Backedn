using Finbuckle.MultiTenant;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using RemaxManagement.Shared.MultiTenant;

namespace RemaxManagement.Shared.Extensions;

public static class MultiTenantExtensions
{
    /// <summary>
    /// Configura multi-tenancy per risoluzione tenant (senza gestione)
    /// </summary>
    public static IServiceCollection AddRemaxMultiTenancy(
        this IServiceCollection services, 
        IConfiguration configuration)
    {
        // Cache per tenant resolution
        services.AddMemoryCache();

        // Configurazione Finbuckle MultiTenant
        services.AddMultiTenant<Tenant>()
            .WithHostStrategy()                                       // Risoluzione per dominio
            .WithStore<EFCoreTenantStore>(ServiceLifetime.Scoped);    // Store EF Core custom

        return services;
    }

    /// <summary>
    /// Configura DbContext multi-tenant per un microservizio
    /// </summary>
    public static IServiceCollection AddMultiTenantDatabase<TContext>(
        this IServiceCollection services, 
        IConfiguration configuration) 
        where TContext : DbContext
    {
        services.AddDbContext<TContext>((serviceProvider, options) =>
        {
            var baseConnection = configuration.GetConnectionString("DefaultConnection");
            options.UseNpgsql(baseConnection);

            // Solo in development
            if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development")
            {
                options.EnableSensitiveDataLogging();
                options.EnableDetailedErrors();
            }
        });

        return services;
    }

    /// <summary>
    /// Rimuove i metodi di migrazione - ora gestiti dal TenantService
    /// I microservizi non devono gestire le migrazioni dei tenant
    /// </summary>
    
    public static string BuildConnectionStringForSchema(string baseConnectionString, string schemaName)
    {
        var builder = new NpgsqlConnectionStringBuilder(baseConnectionString)
        {
            SearchPath = schemaName
        };
        return builder.ToString();
    }
}