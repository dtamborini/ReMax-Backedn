using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Logging;

namespace RemaxManagement.Shared.Data.Extensions;

public static class DatabaseExtensions
{
    public static IServiceCollection AddPostgreSqlDatabase<TContext>(
        this IServiceCollection services, 
        IConfiguration configuration,
        string? schemaName = null) where TContext : DbContext
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        
        if (string.IsNullOrEmpty(connectionString))
        {
            throw new InvalidOperationException("DefaultConnection string not found in configuration");
        }

        services.AddHttpContextAccessor();
        
        services.AddDbContext<TContext>(options =>
        {
            options.UseNpgsql(connectionString, npgsqlOptions =>
            {
                if (!string.IsNullOrEmpty(schemaName))
                {
                    npgsqlOptions.MigrationsHistoryTable("__EFMigrationsHistory", schemaName);
                }
            });
            
            options.EnableSensitiveDataLogging(true);
            options.EnableDetailedErrors(true);
        });

        return services;
    }

    /// <summary>
    /// Applies pending migrations for the specified DbContext
    /// </summary>
    public static WebApplication ApplyMigrations<TContext>(this WebApplication app) where TContext : DbContext
    {
        using var scope = app.Services.CreateScope();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<TContext>>();
        var context = scope.ServiceProvider.GetRequiredService<TContext>();

        try
        {
            logger.LogInformation("Checking for pending migrations for {ContextName}...", typeof(TContext).Name);
            
            var pendingMigrations = context.Database.GetPendingMigrations().ToArray();
            
            if (pendingMigrations.Length > 0)
            {
                logger.LogInformation("Found {Count} pending migrations for {ContextName}: {Migrations}", 
                    pendingMigrations.Length, 
                    typeof(TContext).Name, 
                    string.Join(", ", pendingMigrations));
                
                context.Database.Migrate();
                logger.LogInformation("Successfully applied migrations for {ContextName}", typeof(TContext).Name);
            }
            else
            {
                logger.LogInformation("No pending migrations found for {ContextName}", typeof(TContext).Name);
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error applying migrations for {ContextName}", typeof(TContext).Name);
            throw;
        }

        return app;
    }
}