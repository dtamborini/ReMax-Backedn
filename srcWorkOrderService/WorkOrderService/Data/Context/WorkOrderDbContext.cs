using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using RemaxManagement.Shared.Data.Context;
using WorkOrderService.Data.Entities;

namespace WorkOrderService.Data.Context;

public class WorkOrderDbContext : BaseDbContext
{
    private readonly IHttpContextAccessor? _httpContextAccessor;
    
    public WorkOrderDbContext(DbContextOptions<WorkOrderDbContext> options) : base(options)
    {
    }
    
    public WorkOrderDbContext(DbContextOptions<WorkOrderDbContext> options, IHttpContextAccessor httpContextAccessor) 
        : base(options)
    {
        _httpContextAccessor = httpContextAccessor;
    }
    
    public DbSet<WorkOrder> WorkOrders { get; set; } = null!;
    public DbSet<Intervention> Interventions { get; set; } = null!;
    public DbSet<State> States { get; set; } = null!;
    public DbSet<WorkOrderAttachment> WorkOrderAttachments { get; set; } = null!;
    public DbSet<InterventionAttachment> InterventionAttachments { get; set; } = null!;
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        // Configure WorkOrderAttachment entity
        modelBuilder.Entity<WorkOrderAttachment>(entity =>
        {
            entity.ToTable("WorkOrderAttachments");
            
            // Configure CustomData as JSONB
            entity.Property(e => e.CustomData)
                .HasColumnType("jsonb");
            
            // Configure foreign key relationship to WorkOrder (internal FK)
            entity.HasOne<WorkOrder>()
                .WithMany()
                .HasForeignKey(e => e.WorkOrderId)
                .OnDelete(DeleteBehavior.Cascade);
            
            // Indexes for performance
            entity.HasIndex(e => e.WorkOrderId);
            entity.HasIndex(e => e.AttachmentId); // Cross-service FK - index only
            entity.HasIndex(e => new { e.WorkOrderId, e.AttachmentId }).IsUnique();
        });
        
        // Configure InterventionAttachment entity
        modelBuilder.Entity<InterventionAttachment>(entity =>
        {
            entity.ToTable("InterventionAttachments");
            
            // Configure CustomData as JSONB
            entity.Property(e => e.CustomData)
                .HasColumnType("jsonb");
            
            // Configure foreign key relationship to Intervention (internal FK)
            entity.HasOne<Intervention>()
                .WithMany()
                .HasForeignKey(e => e.InterventionId)
                .OnDelete(DeleteBehavior.Cascade);
            
            // Indexes for performance
            entity.HasIndex(e => e.InterventionId);
            entity.HasIndex(e => e.AttachmentId); // Cross-service FK - index only
            entity.HasIndex(e => new { e.InterventionId, e.AttachmentId }).IsUnique();
        });
        
        // Tutti usano lo schema public (default)
    }
    
    protected override string? GetCurrentUser()
    {
        return _httpContextAccessor?.HttpContext?.User?.Identity?.Name ?? base.GetCurrentUser();
    }
}