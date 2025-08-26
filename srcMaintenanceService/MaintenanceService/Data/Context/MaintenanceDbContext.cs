using MaintenanceService.Data.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Finbuckle.MultiTenant;
using RemaxManagement.Shared.Data.Context;

namespace MaintenanceService.Data.Context;

public class MaintenanceDbContext : BaseDbContext
{
    private readonly IHttpContextAccessor? _httpContextAccessor;
    
    public MaintenanceDbContext(DbContextOptions<MaintenanceDbContext> options) : base(options)
    {
    }
    
    public MaintenanceDbContext(DbContextOptions<MaintenanceDbContext> options, IHttpContextAccessor httpContextAccessor) 
        : base(options)
    {
        _httpContextAccessor = httpContextAccessor;
    }
    
    public MaintenanceDbContext(DbContextOptions<MaintenanceDbContext> options, ITenantInfo tenantInfo) 
        : base(options, tenantInfo)
    {
    }
    
    public MaintenanceDbContext(DbContextOptions<MaintenanceDbContext> options, IHttpContextAccessor httpContextAccessor, ITenantInfo tenantInfo) 
        : base(options, tenantInfo)
    {
        _httpContextAccessor = httpContextAccessor;
    }
    
    public DbSet<MaintenancePlans> MaintenancePlans { get; set; }
    public DbSet<Deadline> Deadlines { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        // FK interna: Deadline -> MaintenancePlans
        modelBuilder.Entity<Deadline>()
            .HasOne<MaintenancePlans>()
            .WithMany()
            .HasForeignKey(d => d.MaintenancePlanId)
            .HasConstraintName("FK_Deadlines_MaintenancePlans_MaintenancePlanId")
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired();
            
        // Indici per FK cross-microservice (solo performance)
        modelBuilder.Entity<MaintenancePlans>()
            .HasIndex(m => m.SupplierId)
            .HasDatabaseName("IX_MaintenancePlans_SupplierId");
            
        modelBuilder.Entity<MaintenancePlans>()
            .HasIndex(m => m.PremisesBuildingId)
            .HasDatabaseName("IX_MaintenancePlans_PremisesBuildingId");
        
        // Tutti usano lo schema public (default)
    }
    
    protected override string? GetCurrentUser()
    {
        return _httpContextAccessor?.HttpContext?.User?.Identity?.Name ?? base.GetCurrentUser();
    }
}