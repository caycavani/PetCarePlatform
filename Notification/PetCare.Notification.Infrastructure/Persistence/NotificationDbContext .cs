using Microsoft.EntityFrameworkCore;
using PetCare.Notification.Domain.Entities;

namespace PetCare.Notification.Infrastructure.Persistence
{
    public class NotificationDbContext : DbContext
    {
        public NotificationDbContext(DbContextOptions<NotificationDbContext> options)
            : base(options) { }

        public DbSet<RNotification> Notifications => Set<RNotification>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<RNotification>(entity =>
            {
                entity.ToTable("Notifications");
                entity.HasKey(n => n.Id);
                entity.Property(n => n.RecipientId).IsRequired();
                entity.Property(n => n.Message).IsRequired().HasMaxLength(500);
                entity.Property(n => n.SentAt).IsRequired();
            });
        }
    }
}
