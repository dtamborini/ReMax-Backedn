using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using RemaxManagement.Shared.Data.Context;
using CommunicationService.Data.Entities;

namespace CommunicationService.Data.Context;

public class CommunicationDbContext : BaseDbContext
{
    private readonly IHttpContextAccessor? _httpContextAccessor;
    
    public CommunicationDbContext(DbContextOptions<CommunicationDbContext> options) : base(options)
    {
    }
    
    public CommunicationDbContext(DbContextOptions<CommunicationDbContext> options, IHttpContextAccessor httpContextAccessor) 
        : base(options)
    {
        _httpContextAccessor = httpContextAccessor;
    }
    
    public DbSet<Communication> Communications { get; set; } = null!;
    public DbSet<CommunicationAttachment> CommunicationAttachments { get; set; } = null!;
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        // Configure CommunicationAttachment entity
        modelBuilder.Entity<CommunicationAttachment>(entity =>
        {
            entity.ToTable("CommunicationAttachments");
            
            // Configure CustomData as JSONB
            entity.Property(e => e.CustomData)
                .HasColumnType("jsonb");
            
            // Indexes for performance
            entity.HasIndex(e => e.CommunicationId);
            entity.HasIndex(e => e.AttachmentId);
            entity.HasIndex(e => new { e.CommunicationId, e.AttachmentId }).IsUnique();
        });
        
        // Tutti usano lo schema public (default)
    }
    
    protected override string? GetCurrentUser()
    {
        return _httpContextAccessor?.HttpContext?.User?.Identity?.Name ?? base.GetCurrentUser();
    }
}