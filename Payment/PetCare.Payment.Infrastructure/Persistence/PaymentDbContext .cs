using Microsoft.EntityFrameworkCore;
using PetCare.Payment.Domain.Entities;

namespace PetCare.Payment.Infrastructure.Persistence
{
    public class PaymentDbContext : DbContext
    {
        public PaymentDbContext(DbContextOptions<PaymentDbContext> options)
            : base(options) { }

        public DbSet<Pay> Payments => Set<Pay>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Pay>(entity =>
            {
                entity.ToTable("Payments");
                entity.HasKey(p => p.Id);
                entity.Property(p => p.ReservationId).IsRequired();
                entity.Property(p => p.Amount).IsRequired().HasColumnType("decimal(18,2)");
                entity.Property(p => p.Method).IsRequired().HasMaxLength(50);
                entity.Property(p => p.PaidAt).IsRequired();
            });
        }
    }
}
