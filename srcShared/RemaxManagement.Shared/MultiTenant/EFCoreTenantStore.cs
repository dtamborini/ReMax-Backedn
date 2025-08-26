using Finbuckle.MultiTenant;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Npgsql;

namespace RemaxManagement.Shared.MultiTenant;

/// <summary>
/// Custom Finbuckle store che usa connessione diretta al database per risolvere tenant
/// Non dipende dal TenantManagementDbContext per evitare dipendenze circolari
/// </summary>
public class EFCoreTenantStore : IMultiTenantStore<Tenant>
{
    private readonly IMemoryCache _cache;
    private readonly ILogger<EFCoreTenantStore> _logger;
    private readonly IConfiguration _configuration;
    private readonly string _connectionString;

    public EFCoreTenantStore(
        IMemoryCache cache,
        ILogger<EFCoreTenantStore> logger,
        IConfiguration configuration)
    {
        _cache = cache;
        _logger = logger;
        _configuration = configuration;
        _connectionString = configuration.GetConnectionString("DefaultConnection") 
            ?? throw new InvalidOperationException("DefaultConnection not found in configuration");
    }

    public async Task<Tenant?> TryGetAsync(string identifier)
    {
        // identifier può essere:
        // - Un dominio: "condominio-rossi.remax.com"
        // - Un tenant identifier: "condominio-rossi"

        var cacheKey = $"tenant:{identifier}";
        if (_cache.TryGetValue(cacheKey, out Tenant? cached))
        {
            _logger.LogDebug("Tenant resolved from cache: {Identifier}", identifier);
            return cached;
        }

        try
        {
            _logger.LogInformation("Attempting to resolve tenant for identifier: {Identifier}", identifier);
            Tenant? tenant = null;

            // Usa connessione diretta al database per evitare dipendenze circolari
            using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();
            _logger.LogDebug("Database connection opened successfully");

            // 1. Prima prova per dominio (più comune)
            // Considera come dominio anche localhost:porta e IP:porta
          
            _logger.LogDebug("Resolving tenant by domain: {Domain}", identifier);
            
            const string domainQuery = @"
                SELECT t.""TenantId"", t.""Identifier"", t.""Name"", t.""ConnectionString"", 
                       t.""AdminEmail"", t.""Address"", t.""CreatedAt"", t.""IsActive""
                FROM ""Tenants"" t
                INNER JOIN ""TenantDomains"" td ON t.""TenantId"" = td.""TenantId""
                WHERE td.""Domain"" = @domain AND td.""IsActive"" = true AND t.""IsActive"" = true
                LIMIT 1";
            
            using var npgsqlCommand = new NpgsqlCommand(domainQuery, connection);
            npgsqlCommand.Parameters.AddWithValue("@domain", identifier);
            
            using var npgsqlDataReader = await npgsqlCommand.ExecuteReaderAsync();
            if (await npgsqlDataReader.ReadAsync())
            {
                tenant = MapReaderToTenant(npgsqlDataReader);
                _logger.LogDebug("Tenant found by domain: {TenantId}", tenant.TenantId);
            }
            else
            {
                _logger.LogDebug("No tenant found for domain: {Domain}", identifier);
            }
            
            
            // 2. Se non trovato, prova per identifier
            if (tenant == null)
            {
                _logger.LogDebug("Resolving tenant by identifier: {Identifier}", identifier);
                
                const string identifierQuery = @"
                    SELECT ""TenantId"", ""Identifier"", ""Name"", ""ConnectionString"", 
                           ""AdminEmail"", ""Address"", ""CreatedAt"", ""IsActive""
                    FROM ""Tenants""
                    WHERE ""Identifier"" = @identifier AND ""IsActive"" = true
                    LIMIT 1";
                
                using var command = new NpgsqlCommand(identifierQuery, connection);
                command.Parameters.AddWithValue("@identifier", identifier);
                
                using var reader = await command.ExecuteReaderAsync();
                if (await reader.ReadAsync())
                {
                    tenant = MapReaderToTenant(reader);
                }
            }

            if (tenant != null)
            {
                // Costruisci connection string con schema del tenant
                tenant.ConnectionString = BuildConnectionStringForTenant(tenant.TenantId.ToString("N"));
                
                // Cache per 10 minuti
                _cache.Set(cacheKey, tenant, TimeSpan.FromMinutes(10));
                
                _logger.LogInformation("Tenant resolved: {TenantId} ({TenantName}) for identifier: {Identifier}", 
                    tenant.TenantId, tenant.Name, identifier);
            }
            else
            {
                _logger.LogWarning("No tenant found for identifier: {Identifier}", identifier);
            }

            return tenant;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error resolving tenant for identifier: {Identifier}", identifier);
            return null;
        }
    }

    public async Task<Tenant?> TryGetByIdentifierAsync(string identifier) 
        => await TryGetAsync(identifier);

    public async Task<IEnumerable<Tenant>> GetAllAsync()
    {
        // Solo per lettura - i microservizi non gestiscono tenant
        try
        {
            using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();
            
            const string query = @"
                SELECT ""TenantId"", ""Identifier"", ""Name"", ""ConnectionString"", 
                       ""AdminEmail"", ""Address"", ""CreatedAt"", ""IsActive""
                FROM ""Tenants""
                WHERE ""IsActive"" = true
                ORDER BY ""Name""";
            
            using var command = new NpgsqlCommand(query, connection);
            using var reader = await command.ExecuteReaderAsync();
            
            var tenants = new List<Tenant>();
            while (await reader.ReadAsync())
            {
                tenants.Add(MapReaderToTenant(reader));
            }
            
            return tenants;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting all tenants");
            return Enumerable.Empty<Tenant>();
        }
    }

    // I metodi di scrittura non sono supportati - solo TenantService può modificare tenant
    public Task<bool> TryAddAsync(Tenant tenantInfo)
        => throw new NotSupportedException("Use TenantService API to add tenants");

    public Task<bool> TryUpdateAsync(Tenant tenantInfo)
        => throw new NotSupportedException("Use TenantService API to update tenants");

    public Task<bool> TryRemoveAsync(string identifier)
        => throw new NotSupportedException("Use TenantService API to remove tenants");

    private static Tenant MapReaderToTenant(NpgsqlDataReader reader)
    {
        var tenantId = reader.GetGuid(reader.GetOrdinal("TenantId"));
        
        return new Tenant
        {
            TenantId = tenantId,
            Id = tenantId.ToString(), // Per ITenantInfo
            Identifier = reader["Identifier"]?.ToString() ?? string.Empty,
            Name = reader["Name"]?.ToString() ?? string.Empty,
            ConnectionString = reader["ConnectionString"]?.ToString() ?? string.Empty,
            AdminEmail = reader["AdminEmail"]?.ToString() ?? string.Empty,
            Address = reader["Address"]?.ToString() ?? string.Empty,
            CreatedAt = DateTime.TryParse(reader["CreatedAt"]?.ToString(), out var createdAt) ? createdAt : DateTime.UtcNow,
            IsActive = bool.TryParse(reader["IsActive"]?.ToString(), out var isActive) ? isActive : true
        };
    }

    private string BuildConnectionStringForTenant(string tenantId)
    {
        var baseConnectionString = _configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("DefaultConnection not found in configuration");
            
        var schemaName = $"tenant_{tenantId.Replace("tenant_", "")}";
        
        var builder = new NpgsqlConnectionStringBuilder(baseConnectionString)
        {
            SearchPath = schemaName
        };
        
        return builder.ToString();
    }

    private void InvalidateCache()
    {
        // In una implementazione reale useresti distributed cache con pattern-based invalidation
        // Per ora invalidazione brutale ma efficace
        if (_cache is MemoryCache mc)
        {
            var field = typeof(MemoryCache).GetField("_coherentState", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (field?.GetValue(mc) is object coherentState)
            {
                var entriesCollection = coherentState.GetType()
                    .GetProperty("EntriesCollection", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                if (entriesCollection?.GetValue(coherentState) is System.Collections.IDictionary entries)
                {
                    var keysToRemove = entries.Keys.Cast<object>()
                        .Where(k => k.ToString()?.StartsWith("tenant:") == true)
                        .ToList();

                    foreach (var key in keysToRemove)
                    {
                        mc.Remove(key);
                    }
                    
                    _logger.LogDebug("Invalidated {Count} tenant cache entries", keysToRemove.Count);
                }
            }
        }
    }
}