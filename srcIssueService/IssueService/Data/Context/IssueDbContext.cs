using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Finbuckle.MultiTenant;
using RemaxManagement.Shared.Data.Context;
using IssueService.Data.Entities;

namespace IssueService.Data.Context;

public class IssueDbContext : BaseDbContext
{
    private readonly IHttpContextAccessor? _httpContextAccessor;
    
    public IssueDbContext(DbContextOptions<IssueDbContext> options) : base(options)
    {
    }
    
    public IssueDbContext(DbContextOptions<IssueDbContext> options, IHttpContextAccessor httpContextAccessor) 
        : base(options)
    {
        _httpContextAccessor = httpContextAccessor;
    }
    
    public IssueDbContext(DbContextOptions<IssueDbContext> options, ITenantInfo tenantInfo) 
        : base(options, tenantInfo)
    {
    }
    
    public IssueDbContext(DbContextOptions<IssueDbContext> options, IHttpContextAccessor httpContextAccessor, ITenantInfo tenantInfo) 
        : base(options, tenantInfo)
    {
        _httpContextAccessor = httpContextAccessor;
    }
    
    public DbSet<Issue> Issues { get; set; } = null!;
    public DbSet<IssueAttachment> IssueAttachments { get; set; } = null!;
    public DbSet<IssueWorkSheet> IssueWorkPlans { get; set; } = null!;
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        // Configure IssueAttachment entity
        modelBuilder.Entity<IssueAttachment>(entity =>
        {
            entity.ToTable("IssueAttachments");
            
            // Configure CustomData as JSONB
            entity.Property(e => e.CustomData)
                .HasColumnType("jsonb");
            
            // Configure foreign key relationship to Issue (internal FK)
            entity.HasOne<Issue>()
                .WithMany()
                .HasForeignKey(e => e.IssueId)
                .OnDelete(DeleteBehavior.Cascade);
            
            // Indexes for performance
            entity.HasIndex(e => e.IssueId);
            entity.HasIndex(e => e.AttachmentId); // Cross-service FK - index only
            entity.HasIndex(e => new { e.IssueId, e.AttachmentId }).IsUnique();
        });
        
        // Configure IssueWorkPlans entity
        modelBuilder.Entity<IssueWorkSheet>(entity =>
        {
            entity.ToTable("IssueWorkPlans");
            
            // Configure CustomData as JSONB
            entity.Property(e => e.CustomData)
                .HasColumnType("jsonb");
            
            // Configure foreign key relationship to Issue (internal FK)
            entity.HasOne<Issue>()
                .WithMany()
                .HasForeignKey(e => e.IssueId)
                .OnDelete(DeleteBehavior.Cascade);
            
            // Indexes for performance
            entity.HasIndex(e => e.IssueId);
            entity.HasIndex(e => e.WorkSheetId); // Cross-service FK - index only
            entity.HasIndex(e => new { e.IssueId, e.WorkSheetId }).IsUnique();
        });
        
        // Tutti usano lo schema public (default)
    }
    
    protected override string? GetCurrentUser()
    {
        return _httpContextAccessor?.HttpContext?.User?.Identity?.Name ?? base.GetCurrentUser();
    }
}