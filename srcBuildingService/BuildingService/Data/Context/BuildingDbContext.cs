using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using RemaxManagement.Shared.Data.Context;
using BuildingService.Data.Entities;
using BuildingService.Data.Enums;

namespace BuildingService.Data.Context;

public class BuildingDbContext : BaseDbContext
{
    private readonly IHttpContextAccessor? _httpContextAccessor;
    
    public BuildingDbContext(DbContextOptions<BuildingDbContext> options) : base(options)
    {
    }
    
    public BuildingDbContext(DbContextOptions<BuildingDbContext> options, IHttpContextAccessor httpContextAccessor) 
        : base(options)
    {
        _httpContextAccessor = httpContextAccessor;
    }
    
    public DbSet<Building> Buildings { get; set; } = null!;
    public DbSet<PremisesBuilding> PremisesBuildings { get; set; } = null!;
    public DbSet<IBAN> IBANs { get; set; } = null!;
    public DbSet<BuildingAttachment> BuildingAttachments { get; set; } = null!;
    public DbSet<PolicyAttachment> PolicyAttachments { get; set; } = null!;
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        // Configure Building entity
        modelBuilder.Entity<Building>(entity =>
        {
            entity.ToTable("Buildings");
            
            // Indexes for performance
            entity.HasIndex(e => e.FiscalCode).IsUnique();
            entity.HasIndex(e => e.VatCode).IsUnique();
            entity.HasIndex(e => e.Name);
        });
        
        // Configure JSONB for PostgreSQL
        modelBuilder.Entity<Building>()
            .Property(e => e.CustomData)
            .HasColumnType("jsonb");
            
        // Configure PremisesBuilding entity
        modelBuilder.Entity<PremisesBuilding>(entity =>
        {
            entity.ToTable("PremisesBuildings");
            
            // Configure self-referencing relationship
            entity.HasOne(e => e.Father)
                .WithMany(e => e.Children)
                .HasForeignKey(e => e.FatherId)
                .OnDelete(DeleteBehavior.Restrict);
            
            // Configure relationship with Building
            entity.HasOne(e => e.Building)
                .WithMany()
                .HasForeignKey(e => e.BuildingId)
                .OnDelete(DeleteBehavior.Cascade);
            
            // Configure TypeTags as JSONB
            entity.Property(e => e.TypeTags)
                .HasColumnType("jsonb")
                .HasDefaultValueSql("'[]'::jsonb");
            
            // Configure CustomData as JSONB
            entity.Property(e => e.CustomData)
                .HasColumnType("jsonb");
                
            // Configure Type enum to be stored as string
            entity.Property(e => e.Type)
                .HasConversion<string>()
                .HasMaxLength(50);
            
            // Indexes for performance
            entity.HasIndex(e => e.BuildingId);
            entity.HasIndex(e => e.FatherId);
            entity.HasIndex(e => e.Type);
            entity.HasIndex(e => new { e.BuildingId, e.Type });
        });
        
        // Configure IBAN entity
        modelBuilder.Entity<IBAN>(entity =>
        {
            entity.ToTable("IBANs");
            
            // Configure relationship with Building
            entity.HasOne(e => e.Building)
                .WithMany()
                .HasForeignKey(e => e.BuildingId)
                .OnDelete(DeleteBehavior.Cascade);
            
            // Configure CustomData as JSONB
            entity.Property(e => e.CustomData)
                .HasColumnType("jsonb");
                
            // Indexes for performance
            entity.HasIndex(e => e.BuildingId);
            entity.HasIndex(e => e.Code);
            entity.HasIndex(e => new { e.BuildingId, e.Code }).IsUnique();
        });
        
        // Configure BuildingAttachment entity
        modelBuilder.Entity<BuildingAttachment>(entity =>
        {
            entity.ToTable("BuildingAttachments");
            
            // Configure self-referencing relationship
            entity.HasOne<BuildingAttachment>()
                .WithMany()
                .HasForeignKey(e => e.FatherId)
                .OnDelete(DeleteBehavior.Restrict);
            
            // Configure CustomData as JSONB
            entity.Property(e => e.CustomData)
                .HasColumnType("jsonb");
                
            // Configure Type enum to be stored as string
            entity.Property(e => e.Type)
                .HasConversion<string>()
                .HasMaxLength(20);
            
            // Indexes for performance
            entity.HasIndex(e => e.BuildingId);
            entity.HasIndex(e => e.FatherId);
            entity.HasIndex(e => e.Type);
            entity.HasIndex(e => e.AttachmentId);
        });
        
        // Configure PolicyAttachment entity
        modelBuilder.Entity<PolicyAttachment>(entity =>
        {
            entity.ToTable("PolicyAttachments");
            
            // Configure CustomData as JSONB
            entity.Property(e => e.CustomData)
                .HasColumnType("jsonb");
            
            // Indexes for performance
            entity.HasIndex(e => e.BuildingId);
            entity.HasIndex(e => e.AttachmentId);
            entity.HasIndex(e => e.Name);
            entity.HasIndex(e => new { e.BuildingId, e.AttachmentId }).IsUnique();
        });
    }
    
    protected override string? GetCurrentUser()
    {
        return _httpContextAccessor?.HttpContext?.User?.Identity?.Name ?? base.GetCurrentUser();
    }
}