using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Finbuckle.MultiTenant;
using RemaxManagement.Shared.Data.Context;
using WorkSheetService.Data.Entities;

namespace WorkSheetService.Data.Context;

public class WorkSheetDbContext : BaseDbContext
{
    private readonly IHttpContextAccessor? _httpContextAccessor;
    
    public WorkSheetDbContext(DbContextOptions<WorkSheetDbContext> options) : base(options)
    {
    }
    
    public WorkSheetDbContext(DbContextOptions<WorkSheetDbContext> options, IHttpContextAccessor httpContextAccessor) 
        : base(options)
    {
        _httpContextAccessor = httpContextAccessor;
    }
    
    public WorkSheetDbContext(DbContextOptions<WorkSheetDbContext> options, ITenantInfo tenantInfo) 
        : base(options, tenantInfo)
    {
    }
    
    public WorkSheetDbContext(DbContextOptions<WorkSheetDbContext> options, IHttpContextAccessor httpContextAccessor, ITenantInfo tenantInfo) 
        : base(options, tenantInfo)
    {
        _httpContextAccessor = httpContextAccessor;
    }
    
    public DbSet<WorkSheets> WorkSheets { get; set; } = null!;
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        // Configurazione WorkSheets
        modelBuilder.Entity<WorkSheets>(entity =>
        {
            // Indice sulla FK BuildingId per performance
            entity.HasIndex(w => w.BuildingId)
                  .HasDatabaseName("IX_WorkSheets_BuildingId");
                  
            // BuildingId è required
            entity.Property(w => w.BuildingId)
                  .IsRequired();
        });
        
        // Per FK cross-microservice useremo SQL raw nelle migrazioni
        // HasOne/WithMany funziona solo per entità nello stesso DbContext
        
        // Tutti usano lo schema public (default)
    }
    
    protected override string? GetCurrentUser()
    {
        return _httpContextAccessor?.HttpContext?.User?.Identity?.Name ?? base.GetCurrentUser();
    }
}