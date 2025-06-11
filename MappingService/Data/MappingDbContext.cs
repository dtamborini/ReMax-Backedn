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

            modelBuilder.Entity<EntityMapping>().HasKey(e => e.Guid);

            modelBuilder.Entity<EntityMapping>(entity =>
            {
                entity.Property(e => e.RowVersion)
                      .IsRowVersion()
                      .ValueGeneratedOnAddOrUpdate()
                      .IsConcurrencyToken();

                entity.Property(e => e.RowVersion)
                      .Metadata.SetBeforeSaveBehavior(Microsoft.EntityFrameworkCore.Metadata.PropertySaveBehavior.Ignore);
            });

            base.OnModelCreating(modelBuilder);
        }
    }
}