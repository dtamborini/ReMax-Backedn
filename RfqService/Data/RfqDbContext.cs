using Microsoft.EntityFrameworkCore;
using RfqService.Models;

namespace RfqService.Data
{
    public class RfqDbContext : DbContext
    {
        public RfqDbContext(DbContextOptions<RfqDbContext> options)
            : base(options)
        {
        }

        public DbSet<Rfq> Rfqs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<Rfq>(entity =>
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