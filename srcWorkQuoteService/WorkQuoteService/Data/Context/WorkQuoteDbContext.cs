using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using RemaxManagement.Shared.Data.Context;

namespace WorkQuoteService.Data.Context;

public class WorkQuoteDbContext : BaseDbContext
{
    private readonly IHttpContextAccessor? _httpContextAccessor;
    
    public WorkQuoteDbContext(DbContextOptions<WorkQuoteDbContext> options) : base(options)
    {
    }
    
    public WorkQuoteDbContext(DbContextOptions<WorkQuoteDbContext> options, IHttpContextAccessor httpContextAccessor) 
        : base(options)
    {
        _httpContextAccessor = httpContextAccessor;
    }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        // Configure schema for work quotes
        // Future entities will be configured here with work_quotes schema
        // Example: modelBuilder.Entity<WorkQuote>().ToTable("WorkQuotes", "work_quotes");
        
        // Configure JSONB for CustomData when entities are added
        // Example: modelBuilder.Entity<WorkQuote>().Property(e => e.CustomData).HasColumnType("jsonb");
    }
    
    protected override string? GetCurrentUser()
    {
        return _httpContextAccessor?.HttpContext?.User?.Identity?.Name ?? base.GetCurrentUser();
    }
}