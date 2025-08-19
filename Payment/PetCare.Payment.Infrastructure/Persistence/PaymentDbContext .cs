using Microsoft.EntityFrameworkCore;
using PetCare.Payment.Domain.Entities;

namespace PetCare.Payment.Infrastructure.Persistence
{
    public class PaymentDbContext : DbContext
    {
        public PaymentDbContext(DbContextOptions<PaymentDbContext> options)
            : base(options) { }

        public DbSet<Pay> Payments => Set<Pay>();
        public DbSet<PaymentMethod> PaymentMethods => Set<PaymentMethod>();
        public DbSet<PaymentStatus> PaymentStatuses => Set<PaymentStatus>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Pay
            modelBuilder.Entity<Pay>(entity =>
            {
                entity.ToTable("Payments");
                entity.HasKey(p => p.Id);

                entity.Property(p => p.ReservationId).IsRequired();
                entity.Property(p => p.Amount).IsRequired().HasColumnType("decimal(18,2)");
                entity.Property(p => p.CreatedAt).IsRequired();
                entity.Property(p => p.CompletedAt);

                entity.Property(p => p.TransactionId);
                entity.Property(p => p.IsSuccessful).IsRequired();
                entity.Property(p => p.LastUpdated).IsRequired();
                entity.Property(p => p.GatewayResponse).HasColumnType("nvarchar(max)");

                entity.HasOne(p => p.Method)
                      .WithMany()
                      .HasForeignKey(p => p.PaymentMethodId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(p => p.Status)
                      .WithMany()
                      .HasForeignKey(p => p.PaymentStatusId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // PaymentMethod
            modelBuilder.Entity<PaymentMethod>(entity =>
            {
                entity.ToTable("PaymentMethods");
                entity.HasKey(pm => pm.Id);
                entity.Property(pm => pm.Name).IsRequired().HasMaxLength(50);
            });

            // PaymentStatus
            modelBuilder.Entity<PaymentStatus>(entity =>
            {
                entity.ToTable("PaymentStatuses");
                entity.HasKey(ps => ps.Id);
                entity.Property(ps => ps.Name).IsRequired().HasMaxLength(50);
            });

            // Seed inicial
            modelBuilder.Entity<PaymentMethod>().HasData(
                new PaymentMethod(1, "CreditCard"),
                new PaymentMethod(2, "Cash"),
                new PaymentMethod(3, "Nequi"),
                new PaymentMethod(4, "Daviplata"),
                new PaymentMethod(5, "BankTransfer"),
                new PaymentMethod(99, "Other")
            );

            modelBuilder.Entity<PaymentStatus>().HasData(
                new PaymentStatus(1, "Pending"),
                new PaymentStatus(2, "Completed"),
                new PaymentStatus(3, "Failed"),
                new PaymentStatus(4, "Refunded")
            );
        }
    }
}
