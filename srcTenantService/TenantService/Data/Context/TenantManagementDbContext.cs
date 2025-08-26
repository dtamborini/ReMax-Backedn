using Microsoft.EntityFrameworkCore;
using RemaxManagement.Shared.MultiTenant;
using RemaxManagement.Shared.Models;

namespace TenantService.Data.Context;

/// <summary>
/// DbContext per gestione tenant - opera su schema pubblico
/// Simile alle SHARED_APPS di django-tenants
/// </summary>
public class TenantManagementDbContext : DbContext
{
    public TenantManagementDbContext(DbContextOptions<TenantManagementDbContext> options) : base(options) { }

    public DbSet<Tenant> Tenants { get; set; }
    public DbSet<TenantDomain> TenantDomains { get; set; }
    public DbSet<SuperAdmin> SuperAdmins { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Forza schema pubblico per metadata tenant
        modelBuilder.HasDefaultSchema("public");
        
        modelBuilder.Entity<Tenant>(entity =>
        {
            entity.ToTable("Tenants");
            
            // Usa TenantId come chiave primaria
            entity.HasKey(t => t.TenantId);
            entity.Property(t => t.TenantId).ValueGeneratedOnAdd();
            
            // Ignora Id (usato solo per ITenantInfo)
            entity.Ignore(t => t.Id);
            
            entity.Property(t => t.Identifier).IsRequired().HasMaxLength(100);
            entity.Property(t => t.Name).IsRequired().HasMaxLength(200);
            entity.Property(t => t.ConnectionString).IsRequired();
            entity.Property(t => t.AdminEmail).HasMaxLength(200);
            entity.Property(t => t.Address).HasMaxLength(500);
            
            // Indici
            entity.HasIndex(t => t.Identifier).IsUnique();
            entity.HasIndex(t => t.IsActive);
        });

        modelBuilder.Entity<TenantDomain>(entity =>
        {
            entity.ToTable("TenantDomains");
            entity.HasKey(td => td.Id);
            entity.Property(td => td.Id).ValueGeneratedOnAdd();
            entity.Property(td => td.TenantId).IsRequired();
            entity.Property(td => td.Domain).IsRequired().HasMaxLength(200);
            
            // Un dominio può appartenere solo a un tenant
            entity.HasIndex(td => td.Domain).IsUnique();
            entity.HasIndex(td => new { td.TenantId, td.IsPrimary });
            entity.HasIndex(td => td.IsActive);
            
            // Relazione 1:N con Tenant
            entity.HasOne(td => td.Tenant)
                  .WithMany(t => t.Domains)
                  .HasForeignKey(td => td.TenantId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<SuperAdmin>(entity =>
        {
            entity.ToTable("SuperAdmins");
            entity.HasKey(sa => sa.Id);
            entity.Property(sa => sa.Id).ValueGeneratedOnAdd();
            
            entity.Property(sa => sa.Username).IsRequired().HasMaxLength(100);
            entity.Property(sa => sa.Email).IsRequired().HasMaxLength(255);
            entity.Property(sa => sa.PasswordHash).IsRequired().HasMaxLength(255);
            entity.Property(sa => sa.FullName).HasMaxLength(100);
            
            // Indici
            entity.HasIndex(sa => sa.Username).IsUnique();
            entity.HasIndex(sa => sa.Email).IsUnique();
            entity.HasIndex(sa => sa.IsActive);
            
            // Ignora la proprietà Role poiché è calcolata
            entity.Ignore(sa => sa.Role);
        });
    }
}