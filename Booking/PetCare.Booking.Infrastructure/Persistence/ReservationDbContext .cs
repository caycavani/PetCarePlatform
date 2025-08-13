using Microsoft.EntityFrameworkCore;
using PetCare.Booking.Domain.Entities;

namespace PetCare.Booking.Infrastructure.Persistence
{
    public class ReservationDbContext : DbContext
    {
        public ReservationDbContext(DbContextOptions<ReservationDbContext> options)
            : base(options) { }

        // 🗂️ Entidades propias del microservicio de reservas
        public DbSet<Reservation> Reservations => Set<Reservation>();
        public DbSet<ReservationStatus> ReservationStatuses => Set<ReservationStatus>();
        public DbSet<Service> Services => Set<Service>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // 🧩 Configuración de Reservation
            modelBuilder.Entity<Reservation>(entity =>
            {
                entity.ToTable("Reservations");

                entity.HasKey(r => r.Id);

                entity.Property(r => r.PetId).IsRequired(); // 👉 Referencia externa
                entity.Property(r => r.ClientId).IsRequired(); // 👉 Referencia externa
                entity.Property(r => r.StartDate).IsRequired();
                entity.Property(r => r.EndDate).IsRequired();
                entity.Property(r => r.ReservationStatusId).IsRequired();
                entity.Property(r => r.Note).HasMaxLength(500);
                entity.Property(r => r.ServiceId).IsRequired();

                entity.HasOne(r => r.Status)
                      .WithMany()
                      .HasForeignKey(r => r.ReservationStatusId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(r => r.Service)
                      .WithMany(s => s.Reservations)
                      .HasForeignKey(r => r.ServiceId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // 🧩 Configuración de Service
            modelBuilder.Entity<Service>(entity =>
            {
                entity.ToTable("Services");

                entity.HasKey(s => s.Id);

                entity.Property(s => s.Name).IsRequired().HasMaxLength(100);
                entity.Property(s => s.Description).HasMaxLength(500);
                entity.Property(s => s.Price).HasColumnType("decimal(18,2)");
                entity.Property(s => s.Duration).IsRequired();
            });

            // 🎨 Estados de reserva preconfigurados
            modelBuilder.Entity<ReservationStatus>().HasData(
                new ReservationStatus { Id = 1, Name = "Pending", DisplayName = "Pendiente", ColorHex = "#ffc107" },
                new ReservationStatus { Id = 2, Name = "Accepted", DisplayName = "Aceptada", ColorHex = "#28a745" },
                new ReservationStatus { Id = 3, Name = "Rejected", DisplayName = "Rechazada", ColorHex = "#dc3545" },
                new ReservationStatus { Id = 4, Name = "Cancelled", DisplayName = "Cancelada", ColorHex = "#6c757d" },
                new ReservationStatus { Id = 5, Name = "Completed", DisplayName = "Completada", ColorHex = "#007bff" }
            );
        }
    }
}
