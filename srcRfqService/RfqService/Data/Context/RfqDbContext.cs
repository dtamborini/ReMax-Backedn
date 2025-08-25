using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using RemaxManagement.Shared.Data.Context;
using RfqService.Data.Entities;

namespace RfqService.Data.Context;

public class RfqDbContext : BaseDbContext
{
    private readonly IHttpContextAccessor? _httpContextAccessor;
    
    public RfqDbContext(DbContextOptions<RfqDbContext> options) : base(options)
    {
    }
    
    public RfqDbContext(DbContextOptions<RfqDbContext> options, IHttpContextAccessor httpContextAccessor) 
        : base(options)
    {
        _httpContextAccessor = httpContextAccessor;
    }
    
    public DbSet<RFQ> RFQs { get; set; } = null!;
    public DbSet<RfqSupplier> RfqSuppliers { get; set; } = null!;
    public DbSet<Quotes> Quotes { get; set; } = null!;
    public DbSet<Negotiation> Negotiations { get; set; } = null!;
    public DbSet<QuoteAttachment> QuoteAttachments { get; set; } = null!;
    public DbSet<NegotiationAttachment> NegotiationAttachments { get; set; } = null!;
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        // Configure RFQ entity
        modelBuilder.Entity<RFQ>(entity =>
        {
            entity.ToTable("RFQs");
            
            // Configure CustomData as JSONB
            entity.Property(e => e.CustomData)
                .HasColumnType("jsonb");
                
            // Configure Priority enum to be stored as string
            entity.Property(e => e.Priority)
                .HasConversion<string>()
                .HasMaxLength(20);
            
            // Indexes for cross-microservice FKs (performance only)
            entity.HasIndex(e => e.BuildingId)
                .HasDatabaseName("IX_RFQs_BuildingId");
            entity.HasIndex(e => e.WorkSheetId)
                .HasDatabaseName("IX_RFQs_WorkSheetId");
            entity.HasIndex(e => e.WorkOrderId)
                .HasDatabaseName("IX_RFQs_WorkOrderId");
            entity.HasIndex(e => e.Priority)
                .HasDatabaseName("IX_RFQs_Priority");
        });
        
        // Configure RfqSupplier entity
        modelBuilder.Entity<RfqSupplier>(entity =>
        {
            entity.ToTable("RfqSuppliers");
            
            // Configure CustomData as JSONB
            entity.Property(e => e.CustomData)
                .HasColumnType("jsonb");
            
            // Configure foreign key relationship to RFQ (internal FK)
            entity.HasOne(rs => rs.RFQ)
                .WithMany()
                .HasForeignKey(rs => rs.RfqId)
                .HasConstraintName("FK_RfqSuppliers_RFQs_RfqId")
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired();
            
            // Index for cross-microservice FK
            entity.HasIndex(e => e.SupplierId)
                .HasDatabaseName("IX_RfqSuppliers_SupplierId");
                
            // Unique constraint to prevent duplicate supplier per RFQ
            entity.HasIndex(e => new { e.RfqId, e.SupplierId })
                .IsUnique()
                .HasDatabaseName("IX_RfqSuppliers_RfqId_SupplierId");
        });
        
        // Configure QuoteAttachment entity
        modelBuilder.Entity<QuoteAttachment>(entity =>
        {
            entity.ToTable("QuoteAttachments");
            
            // Configure CustomData as JSONB
            entity.Property(e => e.CustomData)
                .HasColumnType("jsonb");
            
            // Configure foreign key relationship to Quotes (internal FK)
            entity.HasOne<Quotes>()
                .WithMany()
                .HasForeignKey(e => e.QuoteId)
                .OnDelete(DeleteBehavior.Cascade);
            
            // Indexes for performance
            entity.HasIndex(e => e.QuoteId);
            entity.HasIndex(e => e.AttachmentId); // Cross-service FK - index only
            entity.HasIndex(e => new { e.QuoteId, e.AttachmentId }).IsUnique();
        });
        
        // Configure NegotiationAttachment entity
        modelBuilder.Entity<NegotiationAttachment>(entity =>
        {
            entity.ToTable("NegotiationAttachments");
            
            // Configure CustomData as JSONB
            entity.Property(e => e.CustomData)
                .HasColumnType("jsonb");
            
            // Configure foreign key relationship to Negotiation (internal FK)
            entity.HasOne<Negotiation>()
                .WithMany()
                .HasForeignKey(e => e.NegotiationId)
                .OnDelete(DeleteBehavior.Cascade);
            
            // Indexes for performance
            entity.HasIndex(e => e.NegotiationId);
            entity.HasIndex(e => e.AttachmentId); // Cross-service FK - index only
            entity.HasIndex(e => new { e.NegotiationId, e.AttachmentId }).IsUnique();
        });
        
        // Tutti usano lo schema public (default)
    }
    
    protected override string? GetCurrentUser()
    {
        return _httpContextAccessor?.HttpContext?.User?.Identity?.Name ?? base.GetCurrentUser();
    }
}