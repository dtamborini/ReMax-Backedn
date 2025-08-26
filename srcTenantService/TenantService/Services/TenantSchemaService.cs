using Microsoft.EntityFrameworkCore;
using Npgsql;
using RemaxManagement.Shared.Extensions;
using RemaxManagement.Shared.MultiTenant;
using Microsoft.Extensions.DependencyInjection;
using Finbuckle.MultiTenant;
using System.Diagnostics;
using System.Text;

namespace TenantService.Services;

public interface ITenantSchemaService
{
    Task CreateTenantSchemaAsync(Tenant tenant);
    Task DropTenantSchemaAsync(Guid tenantId);
    Task ApplyMigrationsToTenantSchemaAsync(Tenant tenant);
}

public class TenantSchemaService : ITenantSchemaService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<TenantSchemaService> _logger;
    private readonly IServiceProvider _serviceProvider;

    public TenantSchemaService(
        IConfiguration configuration, 
        ILogger<TenantSchemaService> logger,
        IServiceProvider serviceProvider)
    {
        _configuration = configuration;
        _logger = logger;
        _serviceProvider = serviceProvider;
    }

    public async Task CreateTenantSchemaAsync(Tenant tenant)
    {
        var connectionString = _configuration.GetConnectionString("DefaultConnection");
        if (string.IsNullOrEmpty(connectionString))
            throw new InvalidOperationException("Connection string not found");

        using var connection = new NpgsqlConnection(connectionString);
        await connection.OpenAsync();

        var schemaName = tenant.SchemaName;
        
        _logger.LogInformation("Creating schema {SchemaName} for tenant {TenantId}", schemaName, tenant.TenantId);

        // Crea lo schema
        var createSchemaCommand = new NpgsqlCommand($@"CREATE SCHEMA IF NOT EXISTS ""{schemaName}""", connection);
        await createSchemaCommand.ExecuteNonQueryAsync();

        _logger.LogInformation("Schema {SchemaName} created successfully", schemaName);
    }

    public async Task DropTenantSchemaAsync(Guid tenantId)
    {
        var connectionString = _configuration.GetConnectionString("DefaultConnection");
        if (string.IsNullOrEmpty(connectionString))
            throw new InvalidOperationException("Connection string not found");

        using var connection = new NpgsqlConnection(connectionString);
        await connection.OpenAsync();

        var schemaName = $"tenant_{tenantId:N}";
        
        _logger.LogInformation("Dropping schema {SchemaName} for tenant {TenantId}", schemaName, tenantId);

        // Elimina lo schema e tutto il suo contenuto
        var dropSchemaCommand = new NpgsqlCommand($@"DROP SCHEMA IF EXISTS ""{schemaName}"" CASCADE", connection);
        await dropSchemaCommand.ExecuteNonQueryAsync();

        _logger.LogInformation("Schema {SchemaName} dropped successfully", schemaName);
    }

    public async Task ApplyMigrationsToTenantSchemaAsync(Tenant tenant)
    {
        _logger.LogInformation("Starting migrations for tenant {TenantId} to schema {SchemaName}", 
            tenant.TenantId, tenant.SchemaName);

        var baseConnectionString = _configuration.GetConnectionString("DefaultConnection");
        if (string.IsNullOrEmpty(baseConnectionString))
            throw new InvalidOperationException("Connection string not found");

        // Ensure the schema exists before applying migrations
        await EnsureSchemaExistsAsync(tenant.SchemaName, baseConnectionString);

        // List of all microservice names - we'll apply migrations using dotnet ef directly
        var microservices = new[]
        {
            "AttachmentService",
            "BuildingService", 
            "CheckListService",
            "CommunicationService",
            "InsuranceService",
            "IssueService",
            "MaintenanceService",
            "RfqService",
            "SupplierService",
            "UsersService",
            "WorkOrderService",
            "WorkSheetService"
        };

        var migrationResults = new List<(string Service, bool Success, string? Error)>();

        // Build tenant-specific connection string
        var tenantConnectionString = MultiTenantExtensions.BuildConnectionStringForSchema(baseConnectionString, tenant.SchemaName);

        foreach (var serviceName in microservices)
        {
            try
            {
                _logger.LogInformation("Applying migrations for {ServiceName}", serviceName);
                
                await ApplyMigrationsForService(serviceName, tenantConnectionString);
                
                migrationResults.Add((serviceName, true, null));
                _logger.LogInformation("Successfully applied migrations for {ServiceName}", serviceName);
            }
            catch (Exception ex)
            {
                var errorMessage = $"Failed to apply migrations for {serviceName}: {ex.Message}";
                _logger.LogError(ex, errorMessage);
                migrationResults.Add((serviceName, false, errorMessage));
            }
        }

        // Log summary
        var successfulMigrations = migrationResults.Count(r => r.Success);
        var failedMigrations = migrationResults.Count(r => !r.Success);
        
        _logger.LogInformation(
            "Migration summary for tenant {TenantId}: {SuccessCount} successful, {FailureCount} failed",
            tenant.TenantId, successfulMigrations, failedMigrations);

        if (failedMigrations > 0)
        {
            var failedServices = string.Join(", ", migrationResults.Where(r => !r.Success).Select(r => r.Service));
            throw new InvalidOperationException(
                $"Failed to apply migrations for the following services: {failedServices}. " +
                $"Check logs for detailed error information.");
        }
    }
    

    private async Task EnsureSchemaExistsAsync(string schemaName, string connectionString)
    {
        using var connection = new NpgsqlConnection(connectionString);
        await connection.OpenAsync();

        // Verifica se lo schema esiste
        var checkSchemaCommand = new NpgsqlCommand(@"
            SELECT COUNT(*) 
            FROM information_schema.schemata 
            WHERE schema_name = @schemaName", connection);
        checkSchemaCommand.Parameters.AddWithValue("@schemaName", schemaName);

        var result = await checkSchemaCommand.ExecuteScalarAsync();
        var exists = result != null && (long)result > 0;

        if (!exists)
        {
            _logger.LogInformation("Schema {SchemaName} does not exist, creating it", schemaName);
            
            // Crea lo schema
            var createSchemaCommand = new NpgsqlCommand($@"CREATE SCHEMA ""{schemaName}""", connection);
            await createSchemaCommand.ExecuteNonQueryAsync();
            
            _logger.LogInformation("Schema {SchemaName} created successfully", schemaName);
        }
        else
        {
            _logger.LogInformation("Schema {SchemaName} already exists", schemaName);
        }
    }

    private async Task ApplyMigrationsForService(string serviceName, string tenantConnectionString)
    {
        // Controlla se dobbiamo saltare le migrazioni (per sviluppo locale rapido)
        var skipMigrations = Environment.GetEnvironmentVariable("SKIP_TENANT_MIGRATIONS");
        if (!string.IsNullOrEmpty(skipMigrations) && bool.TryParse(skipMigrations, out var skip) && skip)
        {
            _logger.LogWarning("Skipping migrations for {ServiceName} due to SKIP_TENANT_MIGRATIONS=true", serviceName);
            return;
        }

        _logger.LogInformation("Applying migrations for {ServiceName} using dotnet ef database update", serviceName);

        try
        {
            // Get the path to the service project
            var servicePath = GetServiceProjectPath(serviceName);
            
            if (!Directory.Exists(servicePath))
            {
                throw new DirectoryNotFoundException($"Service path not found: {servicePath}");
            }

            // Prepare the dotnet ef command
            var startInfo = new ProcessStartInfo
            {
                FileName = "dotnet",
                Arguments = $"ef database update --connection \"{tenantConnectionString}\"",
                WorkingDirectory = servicePath,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            _logger.LogInformation("Executing: dotnet ef database update for {ServiceName} in {WorkingDirectory}", 
                serviceName, servicePath);

            using var process = Process.Start(startInfo);
            if (process == null)
            {
                throw new InvalidOperationException($"Failed to start dotnet ef process for {serviceName}");
            }

            var output = new StringBuilder();
            var errors = new StringBuilder();

            // Read output and errors asynchronously
            process.OutputDataReceived += (sender, e) =>
            {
                if (!string.IsNullOrEmpty(e.Data))
                {
                    output.AppendLine(e.Data);
                    _logger.LogInformation("[{ServiceName}] {Output}", serviceName, e.Data);
                }
            };

            process.ErrorDataReceived += (sender, e) =>
            {
                if (!string.IsNullOrEmpty(e.Data))
                {
                    errors.AppendLine(e.Data);
                    _logger.LogWarning("[{ServiceName}] {Error}", serviceName, e.Data);
                }
            };

            process.BeginOutputReadLine();
            process.BeginErrorReadLine();

            // Wait for the process to complete with a timeout
            var completed = await Task.Run(() => process.WaitForExit(300000)); // 5 minutes timeout

            if (!completed)
            {
                process.Kill();
                throw new TimeoutException($"Migration process for {serviceName} timed out after 5 minutes");
            }

            if (process.ExitCode != 0)
            {
                var errorMessage = $"Migration failed for {serviceName}. Exit code: {process.ExitCode}. Errors: {errors}";
                throw new InvalidOperationException(errorMessage);
            }

            _logger.LogInformation("Successfully applied migrations for {ServiceName}", serviceName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error applying migrations for {ServiceName}", serviceName);
            throw;
        }
    }
    
    private string GetServiceProjectPath(string serviceName)
    {
        var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production";
        var forceLocalPath = Environment.GetEnvironmentVariable("FORCE_LOCAL_PROJECT_PATH");
        
        // Se in Development o forzato, usa approccio locale
        if (environment.Equals("Development", StringComparison.OrdinalIgnoreCase) || 
            !string.IsNullOrEmpty(forceLocalPath) && bool.TryParse(forceLocalPath, out var force) && force)
        {
            return GetLocalProjectPath(serviceName);
        }

        // Altrimenti prova approccio Docker prima
        return GetDockerProjectPath(serviceName) ?? GetLocalProjectPath(serviceName);
    }

    private string? GetDockerProjectPath(string serviceName)
    {
        // In produzione (Docker), i progetti sono copiati in /src
        var dockerSourcePath = "/src";
        if (Directory.Exists(dockerSourcePath))
        {
            var servicePath = Path.Combine(dockerSourcePath, $"src{serviceName}", serviceName);
            if (Directory.Exists(servicePath))
            {
                _logger.LogDebug("Using Docker source path for {ServiceName}: {Path}", serviceName, servicePath);
                return servicePath;
            }
        }
        return null;
    }

    private string GetLocalProjectPath(string serviceName)
    {
        // Fallback per sviluppo locale
        var currentDirectory = Directory.GetCurrentDirectory();
        
        // Navigate up to find the solution root
        var directory = new DirectoryInfo(currentDirectory);
        while (directory != null && !directory.GetFiles("*.sln").Any())
        {
            directory = directory.Parent;
        }

        if (directory == null)
        {
            throw new DirectoryNotFoundException($"Could not find solution root directory. Current: {currentDirectory}");
        }

        var localPath = Path.Combine(directory.FullName, $"src{serviceName}", serviceName);
        _logger.LogDebug("Using local development path for {ServiceName}: {Path}", serviceName, localPath);
        
        if (!Directory.Exists(localPath))
        {
            throw new DirectoryNotFoundException($"Local project path not found: {localPath}");
        }
        
        return localPath;
    }
    
}