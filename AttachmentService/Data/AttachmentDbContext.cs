using Microsoft.EntityFrameworkCore;
using AttachmentService.Models;

namespace AttachmentService.Data
{
    public class AttachmentDbContext : DbContext
    {
        public AttachmentDbContext(DbContextOptions<AttachmentDbContext> options)
            : base(options)
        {
        }

        public DbSet<Attachment> Attachments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<Attachment>(entity =>
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