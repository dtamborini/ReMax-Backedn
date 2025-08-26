using CheckListService.Data.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Finbuckle.MultiTenant;
using RemaxManagement.Shared.Data.Context;

namespace CheckListService.Data.Context;

public class CheckListDbContext : BaseDbContext
{
    private readonly IHttpContextAccessor? _httpContextAccessor;
    
    public CheckListDbContext(DbContextOptions<CheckListDbContext> options) : base(options)
    {
    }
    
    public CheckListDbContext(DbContextOptions<CheckListDbContext> options, IHttpContextAccessor httpContextAccessor) 
        : base(options)
    {
        _httpContextAccessor = httpContextAccessor;
    }
    
    public CheckListDbContext(DbContextOptions<CheckListDbContext> options, ITenantInfo tenantInfo) 
        : base(options, tenantInfo)
    {
    }
    
    public CheckListDbContext(DbContextOptions<CheckListDbContext> options, IHttpContextAccessor httpContextAccessor, ITenantInfo tenantInfo) 
        : base(options, tenantInfo)
    {
        _httpContextAccessor = httpContextAccessor;
    }
    
    public DbSet<CheckListGroup> CheckListGroups { get; set; } = null!;
    public DbSet<CheckList> CheckLists { get; set; } = null!;
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        // Configure CheckListGroup entity
        modelBuilder.Entity<CheckListGroup>(entity =>
        {
            entity.ToTable("CheckListGroups");
            
            // Configure CustomData as JSONB
            entity.Property(e => e.CustomData)
                .HasColumnType("jsonb");
            
            // Indexes for performance
            entity.HasIndex(e => e.WorkOrderId);
            entity.HasIndex(e => e.Name);
            entity.HasIndex(e => e.Complete);
        });
        
        // Configure CheckList entity
        modelBuilder.Entity<CheckList>(entity =>
        {
            entity.ToTable("CheckLists");
            
            // Configure CustomData as JSONB
            entity.Property(e => e.CustomData)
                .HasColumnType("jsonb");
                
            // Configure Type enum to be stored as string
            entity.Property(e => e.Type)
                .HasConversion<string>()
                .HasMaxLength(20);
            
            // Configure foreign key relationship to CheckListGroup (internal FK)
            entity.HasOne<CheckListGroup>()
                .WithMany()
                .HasForeignKey(e => e.CheckListGroupId)
                .OnDelete(DeleteBehavior.Cascade);
            
            // Indexes for performance
            entity.HasIndex(e => e.CheckListGroupId);
            entity.HasIndex(e => e.Type);
            entity.HasIndex(e => e.Name);
            entity.HasIndex(e => e.AttachmentId); // Cross-service FK - index only
            entity.HasIndex(e => e.BuildingAttachmentId); // Cross-service FK - index only
            entity.HasIndex(e => e.Required);
        });
        
        // Tutti usano lo schema public (default)
    }
    
    protected override string? GetCurrentUser()
    {
        return _httpContextAccessor?.HttpContext?.User?.Identity?.Name ?? base.GetCurrentUser();
    }
}