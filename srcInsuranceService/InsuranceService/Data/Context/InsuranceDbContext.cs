using InsuranceService.Data.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using RemaxManagement.Shared.Data.Context;

namespace InsuranceService.Data.Context;

public class InsuranceDbContext : BaseDbContext
{
    private readonly IHttpContextAccessor? _httpContextAccessor;
    
    public InsuranceDbContext(DbContextOptions<InsuranceDbContext> options) : base(options)
    {
    }
    
    public InsuranceDbContext(DbContextOptions<InsuranceDbContext> options, IHttpContextAccessor httpContextAccessor) 
        : base(options)
    {
        _httpContextAccessor = httpContextAccessor;
    }
    
    public DbSet<Insurance> Insurances { get; set; }
    public DbSet<InsuranceDeductible> InsuranceDeductibles { get; set; }
    public DbSet<InsuranceLimits> InsuranceLimits { get; set; }
    public DbSet<InsuranceAccidents> InsuranceAccidents { get; set; }
    public DbSet<InsuranceAccidentsIssue> InsuranceAccidentsIssues { get; set; }
    public DbSet<InsuranceAccidentsWorksPlan> InsuranceAccidentsWorksPlans { get; set; }
    
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