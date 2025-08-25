using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using RemaxManagement.Shared.Data.Context;
using SupplierService.Data.Entities;

namespace SupplierService.Data.Context;

public class SupplierDbContext : BaseDbContext
{
    private readonly IHttpContextAccessor? _httpContextAccessor;
    
    public SupplierDbContext(DbContextOptions<SupplierDbContext> options) : base(options)
    {
    }
    
    public SupplierDbContext(DbContextOptions<SupplierDbContext> options, IHttpContextAccessor httpContextAccessor) 
        : base(options)
    {
        _httpContextAccessor = httpContextAccessor;
    }
    
    public DbSet<Supplier> Suppliers { get; set; } = null!;
    public DbSet<SupplierBuilding> SupplierBuildings { get; set; } = null!;
    public DbSet<Service> Services { get; set; } = null!;
    public DbSet<SupplierAttachment> SupplierAttachments { get; set; } = null!;
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        // Configure SupplierBuilding entity
        modelBuilder.Entity<SupplierBuilding>(entity =>
        {
            // FK constraint per SupplierId (interna al microservizio)
            entity.HasOne(sb => sb.Supplier)
                  .WithMany()
                  .HasForeignKey(sb => sb.SupplierId)
                  .HasConstraintName("FK_SupplierBuildings_Suppliers_SupplierId")
                  .OnDelete(DeleteBehavior.Cascade)
                  .IsRequired();
            
            // Indice per BuildingId (cross-microservice, solo performance)
            entity.HasIndex(e => e.BuildingId)
                  .HasDatabaseName("IX_SupplierBuildings_BuildingId");
        });
        
        // Configure SupplierAttachment entity
        modelBuilder.Entity<SupplierAttachment>(entity =>
        {
            entity.ToTable("SupplierAttachments");
            
            // Configure CustomData as JSONB
            entity.Property(e => e.CustomData)
                .HasColumnType("jsonb");
                
            // Configure State enum to be stored as string
            entity.Property(e => e.State)
                .HasConversion<string>()
                .HasMaxLength(20);
            
            // FK constraint per SupplierId (interna al microservizio)
            entity.HasOne<Supplier>()
                  .WithMany()
                  .HasForeignKey(sa => sa.SupplierId)
                  .HasConstraintName("FK_SupplierAttachments_Suppliers_SupplierId")
                  .OnDelete(DeleteBehavior.Cascade)
                  .IsRequired();
            
            // Indexes for performance
            entity.HasIndex(e => e.AttachmentId); // cross-microservice FK
            entity.HasIndex(e => e.State);
            entity.HasIndex(e => e.ExpireDate);
            entity.HasIndex(e => e.Name);
        });
        
        // Tutti usano lo schema public (default)
    }
    
    protected override string? GetCurrentUser()
    {
        return _httpContextAccessor?.HttpContext?.User?.Identity?.Name ?? base.GetCurrentUser();
    }
}