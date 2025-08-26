using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using Finbuckle.MultiTenant;
using RemaxManagement.Shared.Data.Entities;
using RemaxManagement.Shared.MultiTenant;
using System.Linq.Expressions;

namespace RemaxManagement.Shared.Data.Context;

public abstract class BaseDbContext : DbContext
{
    private readonly ITenantInfo? _tenantInfo;
    
    protected BaseDbContext(DbContextOptions options) : base(options)
    {
    }
    
    protected BaseDbContext(DbContextOptions options, ITenantInfo? tenantInfo) : base(options)
    {
        _tenantInfo = tenantInfo;
    }

    public override int SaveChanges()
    {
        UpdateTimestamps();
        return base.SaveChanges();
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        UpdateTimestamps();
        return await base.SaveChangesAsync(cancellationToken);
    }

    private void UpdateTimestamps()
    {
        var entries = ChangeTracker.Entries<BaseEntity>();
        var currentUser = GetCurrentUser(); // Method to be implemented based on your auth system

        foreach (var entry in entries)
        {
            if (entry.State == EntityState.Added)
            {
                entry.Entity.CreatedAt = DateTime.UtcNow;
                entry.Entity.CreatedBy = currentUser;
            }
            else if (entry.State == EntityState.Modified)
            {
                entry.Entity.UpdatedAt = DateTime.UtcNow;
                entry.Entity.UpdatedBy = currentUser;
                
                // Prevent modification of created fields
                entry.Property(x => x.CreatedAt).IsModified = false;
                entry.Property(x => x.CreatedBy).IsModified = false;
            }
            else if (entry.State == EntityState.Deleted && entry.Entity is BaseEntity entity)
            {
                // Convert hard delete to soft delete
                entry.State = EntityState.Modified;
            }
        }
    }
    
    protected virtual string? GetCurrentUser()
    {
        // Override this in derived contexts to provide actual user from HttpContext or similar
        return "system";
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        // Configura schema tenant se disponibile
        ConfigureTenantSchema(modelBuilder);

        // Apply global query filter for soft delete
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            if (typeof(BaseEntity).IsAssignableFrom(entityType.ClrType))
            {
                var parameter = Expression.Parameter(entityType.ClrType, "e");
                var propertyMethodInfo = typeof(EF).GetMethod("Property")?.MakeGenericMethod(typeof(bool));
                var isDeletedProperty = Expression.Call(propertyMethodInfo, parameter, Expression.Constant("IsDeleted"));
                var compareExpression = Expression.MakeBinary(ExpressionType.Equal, isDeletedProperty, Expression.Constant(false));
                var lambda = Expression.Lambda(compareExpression, parameter);

                modelBuilder.Entity(entityType.ClrType).HasQueryFilter(lambda);
            }
        }
    }
    
    private void ConfigureTenantSchema(ModelBuilder modelBuilder)
    {
        // Se abbiamo informazioni sul tenant, usa il suo schema
        if (_tenantInfo is Tenant tenant)
        {
            modelBuilder.HasDefaultSchema(tenant.SchemaName);
        }
        else if (_tenantInfo != null && !string.IsNullOrEmpty(_tenantInfo.Id))
        {
            // Fallback: costruisci schema name dall'ID del tenant
            var schemaName = $"tenant_{_tenantInfo.Id.Replace("-", "")}";
            modelBuilder.HasDefaultSchema(schemaName);
        }
        // Se non c'è tenant info, usa schema pubblico (solo per TenantService)
    }
}