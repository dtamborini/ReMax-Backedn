using Microsoft.EntityFrameworkCore;
using WorkSheetService.Models;

namespace WorkSheetService.Data
{
    public class WorkSheetDbContext : DbContext
    {
        public WorkSheetDbContext(DbContextOptions<WorkSheetDbContext> options)
            : base(options)
        {
        }

        public DbSet<WorkSheet> WorkSheets { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<WorkSheet>(entity =>
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