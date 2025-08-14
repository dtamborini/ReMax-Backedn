using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using RemaxManagement.Shared.Data.Context;

namespace AssetService.Data.Context;

public class AssetDbContext : BaseDbContext
{
    private readonly IHttpContextAccessor? _httpContextAccessor;
    
    public AssetDbContext(DbContextOptions<AssetDbContext> options) : base(options)
    {
    }
    
    public AssetDbContext(DbContextOptions<AssetDbContext> options, IHttpContextAccessor httpContextAccessor) 
        : base(options)
    {
        _httpContextAccessor = httpContextAccessor;
    }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        // Configure schema for assets
        // Future entities will be configured here with assets schema
        // Example: modelBuilder.Entity<Asset>().ToTable("Assets", "assets");
        
        // Configure JSONB for CustomData when entities are added
        // Example: modelBuilder.Entity<Asset>().Property(e => e.CustomData).HasColumnType("jsonb");
    }
    
    protected override string? GetCurrentUser()
    {
        return _httpContextAccessor?.HttpContext?.User?.Identity?.Name ?? base.GetCurrentUser();
    }
}