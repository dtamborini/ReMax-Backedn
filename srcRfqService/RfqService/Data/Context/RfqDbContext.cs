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
        
        // Configure QuoteAttachment entity
        modelBuilder.Entity<QuoteAttachment>(entity =>
        {
            entity.ToTable("QuoteAttachments");
            
            // Configure CustomData as JSONB
            entity.Property(e => e.CustomData)
                .HasColumnType("jsonb");
            
            // Indexes for performance
            entity.HasIndex(e => e.QuoteId);
            entity.HasIndex(e => e.AttachmentId);
            entity.HasIndex(e => new { e.QuoteId, e.AttachmentId }).IsUnique();
        });
        
        // Configure NegotiationAttachment entity
        modelBuilder.Entity<NegotiationAttachment>(entity =>
        {
            entity.ToTable("NegotiationAttachments");
            
            // Configure CustomData as JSONB
            entity.Property(e => e.CustomData)
                .HasColumnType("jsonb");
            
            // Indexes for performance
            entity.HasIndex(e => e.NegotiationId);
            entity.HasIndex(e => e.AttachmentId);
            entity.HasIndex(e => new { e.NegotiationId, e.AttachmentId }).IsUnique();
        });
        
        // Tutti usano lo schema public (default)
    }
    
    protected override string? GetCurrentUser()
    {
        return _httpContextAccessor?.HttpContext?.User?.Identity?.Name ?? base.GetCurrentUser();
    }
}