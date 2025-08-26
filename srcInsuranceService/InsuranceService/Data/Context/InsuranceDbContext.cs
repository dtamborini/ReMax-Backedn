using InsuranceService.Data.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Finbuckle.MultiTenant;
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
    
    public InsuranceDbContext(DbContextOptions<InsuranceDbContext> options, ITenantInfo tenantInfo) 
        : base(options, tenantInfo)
    {
    }
    
    public InsuranceDbContext(DbContextOptions<InsuranceDbContext> options, IHttpContextAccessor httpContextAccessor, ITenantInfo tenantInfo) 
        : base(options, tenantInfo)
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
        
        // Configurazione FK interne al microservizio con HasOne/WithMany
        
        // InsuranceDeductible -> Insurance
        modelBuilder.Entity<InsuranceDeductible>()
            .HasOne<Insurance>()
            .WithMany()
            .HasForeignKey(d => d.InsuranceId)
            .HasConstraintName("FK_InsuranceDeductibles_Insurances_InsuranceId")
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired();
            
        // InsuranceLimits -> Insurance
        modelBuilder.Entity<InsuranceLimits>()
            .HasOne<Insurance>()
            .WithMany()
            .HasForeignKey(l => l.InsuranceId)
            .HasConstraintName("FK_InsuranceLimits_Insurances_InsuranceId")
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired();
            
        // InsuranceAccidents -> Insurance
        modelBuilder.Entity<InsuranceAccidents>()
            .HasOne<Insurance>()
            .WithMany()
            .HasForeignKey(a => a.InsuranceId)
            .HasConstraintName("FK_InsuranceAccidents_Insurances_InsuranceId")
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired();
            
        // InsuranceAccidentsIssue -> InsuranceAccidents
        modelBuilder.Entity<InsuranceAccidentsIssue>()
            .HasOne<InsuranceAccidents>()
            .WithMany()
            .HasForeignKey(i => i.InsuranceAccidentId)
            .HasConstraintName("FK_InsuranceAccidentsIssues_InsuranceAccidents_InsuranceAccidentId")
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired();
            
        // InsuranceAccidentsWorksPlan -> InsuranceAccidents
        modelBuilder.Entity<InsuranceAccidentsWorksPlan>()
            .HasOne<InsuranceAccidents>()
            .WithMany()
            .HasForeignKey(w => w.InsuranceAccidentId)
            .HasConstraintName("FK_InsuranceAccidentsWorksPlans_InsuranceAccidents_InsuranceAccidentId")
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired();
            
        // Indici per FK cross-microservice (solo performance, no constraint)
        modelBuilder.Entity<Insurance>()
            .HasIndex(i => i.PolicyDocumentId)
            .HasDatabaseName("IX_Insurances_PolicyDocumentId");
            
        modelBuilder.Entity<Insurance>()
            .HasIndex(i => i.ReceiptDocumentId)
            .HasDatabaseName("IX_Insurances_ReceiptDocumentId");
            
        modelBuilder.Entity<InsuranceAccidents>()
            .HasIndex(a => a.BuildingId)
            .HasDatabaseName("IX_InsuranceAccidents_BuildingId");
            
        modelBuilder.Entity<InsuranceAccidentsIssue>()
            .HasIndex(i => i.IssueId)
            .HasDatabaseName("IX_InsuranceAccidentsIssues_IssueId");
            
        modelBuilder.Entity<InsuranceAccidentsWorksPlan>()
            .HasIndex(w => w.WorkPlanId)
            .HasDatabaseName("IX_InsuranceAccidentsWorksPlans_WorkPlanId");
        
        // Tutti usano lo schema public (default)
    }
    
    protected override string? GetCurrentUser()
    {
        return _httpContextAccessor?.HttpContext?.User?.Identity?.Name ?? base.GetCurrentUser();
    }
}