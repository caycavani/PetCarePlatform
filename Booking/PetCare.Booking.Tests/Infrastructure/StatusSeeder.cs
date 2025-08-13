using System.Collections.Generic;
using System.Linq;
using PetCare.Booking.Domain.Entities;
using PetCare.Booking.Infrastructure.Persistence;

namespace PetCare.Booking.Tests.Infrastructure
{
    public static class StatusSeeder
    {
        // 🆔 Estado usado para validar reservas accesibles
        public static readonly int EstadoAcceptedId = 2;

        public static void Seed(ReservationDbContext context)
        {
            if (context.ReservationStatuses.Any()) return; // Evita duplicados

            var statuses = new List<ReservationStatus>
            {
                new ReservationStatus(1, "Pending", "Pendiente", "#ffc107"),
                new ReservationStatus(EstadoAcceptedId, "Accepted", "Aceptada", "#28a745"),
                new ReservationStatus(3, "Canceled", "Cancelada", "#6c757d"),
                new ReservationStatus(4, "Finished", "Finalizada", "#007bff")
            };

            context.ReservationStatuses.AddRange(statuses);
            context.SaveChanges();
        }
    }
}
