using Microsoft.EntityFrameworkCore;
using WorkOrderService.Models;

namespace WorkOrderService.Data
{
    public class WorkOrderDbContext : DbContext
    {
        public WorkOrderDbContext(DbContextOptions<WorkOrderDbContext> options)
            : base(options)
        {
        }

        public DbSet<WorkOrder> WorkOrders { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<WorkOrder>(entity =>
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