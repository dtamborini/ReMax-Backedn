using MappingService.Models;
using Microsoft.EntityFrameworkCore;

namespace MappingService.Data
{
    public class MappingDbContext : DbContext
    {
        public MappingDbContext(DbContextOptions<MappingDbContext> options)
            : base(options)
        {
        }

        public DbSet<EntityMapping> Mappings { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<EntityMapping>().HasKey(e => e.Guid);

            modelBuilder.Entity<EntityMapping>()
                .Property(e => e.Type)
                .HasConversion<string>();
        }
    }
}