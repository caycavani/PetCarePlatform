using Microsoft.EntityFrameworkCore;
using PetCare.Notification.Domain.Entities;
using PetCare.Notification.Domain.Value_Objects;
using PetCare.Notification.Domain.Value_Objects.Enums;

namespace PetCare.Notification.Infrastructure.Persistence
{
    public class NotificationDbContext : DbContext
    {
        public NotificationDbContext(DbContextOptions<NotificationDbContext> options)
            : base(options) { }

        public DbSet<Ntification> Notifications => Set<Ntification>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Ntification>(entity =>
            {
                entity.ToTable("Notifications");

                entity.HasKey(n => n.Id);

                // Mapeo de Value Object: Recipient
                entity.OwnsOne(n => n.Recipient, recipient =>
                {
                    recipient.Property(r => r.Name).HasColumnName("RecipientName").IsRequired().HasMaxLength(100);
                    recipient.Property(r => r.Email).HasColumnName("RecipientEmail").IsRequired().HasMaxLength(150);
                    recipient.Property(r => r.PhoneNumber).HasColumnName("RecipientPhoneNumber").HasMaxLength(20);
                    recipient.Property(r => r.DeviceToken).HasColumnName("RecipientDeviceToken").HasMaxLength(200);
                });

                // Mapeo de Value Object: NotificationContent
                entity.OwnsOne(n => n.Content, content =>
                {
                    content.Property(c => c.Subject).HasColumnName("Subject").IsRequired().HasMaxLength(200);
                    content.Property(c => c.Message).HasColumnName("Message").IsRequired().HasMaxLength(1000);
                });

                // Mapeo de Enum
                entity.Property(n => n.Channel)
                      .HasConversion<string>() // Guarda como texto
                      .HasMaxLength(20)
                      .IsRequired();

                entity.Property(n => n.ScheduledAt)
                      .IsRequired();
            });
        }
    }
}
