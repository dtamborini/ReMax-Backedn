using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using RemaxManagement.Shared.Data.Context;
using UsersService.Data.Entities;

namespace UsersService.Data.Context;

public class UsersDbContext : BaseDbContext
{
    private readonly IHttpContextAccessor? _httpContextAccessor;
    
    public UsersDbContext(DbContextOptions<UsersDbContext> options) : base(options)
    {
    }
    
    public UsersDbContext(DbContextOptions<UsersDbContext> options, IHttpContextAccessor httpContextAccessor) 
        : base(options)
    {
        _httpContextAccessor = httpContextAccessor;
    }
    
    public DbSet<Resident> Residents { get; set; } = null!;
    public DbSet<ResidentPremises> ResidentPremises { get; set; } = null!;
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        // Configure Resident entity
        modelBuilder.Entity<Resident>(entity =>
        {
            entity.ToTable("Residents");
            
            // Configure self-referencing relationships
            entity.HasOne(e => e.Delegate)
                .WithMany(e => e.DelegatedFrom)
                .HasForeignKey(e => e.DelegateId)
                .OnDelete(DeleteBehavior.Restrict);
                
            entity.HasOne(e => e.Delegator)
                .WithMany(e => e.DelegatedTo)
                .HasForeignKey(e => e.DelegatorId)
                .OnDelete(DeleteBehavior.Restrict);
            
            // Configure CustomData as JSONB
            entity.Property(e => e.CustomData)
                .HasColumnType("jsonb");
                
            // Indexes for performance
            entity.HasIndex(e => e.Email).IsUnique();
            entity.HasIndex(e => e.Surname);
            entity.HasIndex(e => e.Name);
            entity.HasIndex(e => new { e.Name, e.Surname });
            entity.HasIndex(e => e.DelegateId);
            entity.HasIndex(e => e.DelegatorId);
        });
        
        // Configure ResidentPremises entity
        modelBuilder.Entity<ResidentPremises>(entity =>
        {
            entity.ToTable("ResidentPremises");
            
            // Configure relationship with Resident
            entity.HasOne(e => e.Resident)
                .WithMany(r => r.ResidentPremises)
                .HasForeignKey(e => e.ResidentId)
                .OnDelete(DeleteBehavior.Cascade);
            
            // Configure CustomData as JSONB
            entity.Property(e => e.CustomData)
                .HasColumnType("jsonb");
                
            // Configure Percentage precision
            entity.Property(e => e.Percentage)
                .HasPrecision(18, 4); // Allow up to 4 decimal places
                
            // Indexes for performance
            entity.HasIndex(e => e.ResidentId);
            entity.HasIndex(e => e.BuildingId);
            entity.HasIndex(e => e.PremisesBuildingId);
            entity.HasIndex(e => new { e.ResidentId, e.BuildingId });
            entity.HasIndex(e => new { e.ResidentId, e.PremisesBuildingId });
            
            // Unique constraint: one resident can have only one record per premises
            entity.HasIndex(e => new { e.ResidentId, e.PremisesBuildingId })
                .IsUnique()
                .HasDatabaseName("IX_ResidentPremises_Resident_Premises_Unique");
        });
    }
    
    protected override string? GetCurrentUser()
    {
        return _httpContextAccessor?.HttpContext?.User?.Identity?.Name ?? base.GetCurrentUser();
    }
}