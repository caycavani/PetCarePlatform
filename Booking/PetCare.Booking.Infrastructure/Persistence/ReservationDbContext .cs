using Microsoft.EntityFrameworkCore;
using PetCare.Booking.Domain.Entities;

namespace PetCare.Booking.Infrastructure.Persistence
{
    public class ReservationDbContext : DbContext
    {        public ReservationDbContext(DbContextOptions<ReservationDbContext> options)
            : base(options) { }

        public DbSet<Reservation> Reservations => Set<Reservation>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Reservation>(entity =>
            {
                entity.ToTable("Reservations");
                entity.HasKey(r => r.Id);
                entity.Property(r => r.PetId).IsRequired();
                entity.Property(r => r.CaregiverId).IsRequired();
                entity.Property(r => r.StartDate).IsRequired();
                entity.Property(r => r.EndDate).IsRequired();
                entity.Property(r => r.Status).IsRequired().HasMaxLength(50);
            });
        }
    }
}
