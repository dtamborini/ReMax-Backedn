using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

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
}