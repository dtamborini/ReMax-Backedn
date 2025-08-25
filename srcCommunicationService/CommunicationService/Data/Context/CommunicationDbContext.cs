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
            
            // FK interna: CommunicationAttachment -> Communication
            entity.HasOne<Communication>()
                .WithMany()
                .HasForeignKey(ca => ca.CommunicationId)
                .HasConstraintName("FK_CommunicationAttachments_Communications_CommunicationId")
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired();
            
            // Indexes for performance
            entity.HasIndex(e => e.AttachmentId);
            entity.HasIndex(e => new { e.CommunicationId, e.AttachmentId }).IsUnique();
        });
        
        // Indice per FK cross-microservice
        modelBuilder.Entity<Communication>()
            .HasIndex(c => c.BuildingId)
            .HasDatabaseName("IX_Communications_BuildingId");
        
        // Tutti usano lo schema public (default)
    }
    
    protected override string? GetCurrentUser()
    {
        return _httpContextAccessor?.HttpContext?.User?.Identity?.Name ?? base.GetCurrentUser();
    }
}