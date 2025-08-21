using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PetCare.Notification.Domain.Entities;

namespace PetCare.Notification.Infrastructure.Persistence.Configurations
{
    public class NotificationRecordConfiguration : IEntityTypeConfiguration<Ntification>
    {
        public void Configure(EntityTypeBuilder<Ntification> builder)
        {
            builder.ToTable("Notifications");

            builder.HasKey(n => n.Id);

            builder.OwnsOne(n => n.Recipient, recipient =>
            {
                recipient.Property(r => r.Name).HasColumnName("RecipientName").IsRequired().HasMaxLength(100);
                recipient.Property(r => r.Email).HasColumnName("RecipientEmail").IsRequired().HasMaxLength(150);
                recipient.Property(r => r.PhoneNumber).HasColumnName("RecipientPhoneNumber").HasMaxLength(20);
                recipient.Property(r => r.DeviceToken).HasColumnName("RecipientDeviceToken").HasMaxLength(200);
            });

            builder.OwnsOne(n => n.Content, content =>
            {
                content.Property(c => c.Subject).HasColumnName("Subject").IsRequired().HasMaxLength(200);
                content.Property(c => c.Message).HasColumnName("Message").IsRequired().HasMaxLength(1000);
            });

            builder.Property(n => n.Channel)
                   .HasConversion<string>()
                   .HasMaxLength(20)
                   .IsRequired();

            builder.Property(n => n.ScheduledAt)
                   .IsRequired();
        }
    }
}
