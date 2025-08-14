using MaintenanceService.Data.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
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
    
    public DbSet<MaintenancePlans> MaintenancePlans { get; set; }
    public DbSet<Deadline> Deadlines { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        // Tutti usano lo schema public (default)
    }
    
    protected override string? GetCurrentUser()
    {
        return _httpContextAccessor?.HttpContext?.User?.Identity?.Name ?? base.GetCurrentUser();
    }
}