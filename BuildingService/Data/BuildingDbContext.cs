using Microsoft.EntityFrameworkCore;
using BuildingService.Models;

namespace BuildingService.Data
{
    public class BuildingDbContext : DbContext
    {
        public BuildingDbContext(DbContextOptions<BuildingDbContext> options)
            : base(options)
        {
        }

        public DbSet<Building> Buildings { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<Building>(entity =>
            {
                entity.HasKey(e => e.Guid);

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