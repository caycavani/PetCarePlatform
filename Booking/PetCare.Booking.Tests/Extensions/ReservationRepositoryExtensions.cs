using Microsoft.EntityFrameworkCore;
using PetCare.Booking.Domain.Entities;
using PetCare.Booking.Infrastructure.Persistence;
using PetCare.Booking.Infrastructure.Repositories;

namespace PetCare.Booking.Tests.Extensions
{
    public static class ReservationRepositoryExtensions
    {
        public static async Task<Reservation?> GetRawByIdAsync(this ReservationRepository repository, Guid id)
        {
            var contextField = typeof(ReservationRepository)
                               .GetField("_context", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

            var context = (ReservationDbContext?)contextField?.GetValue(repository);

            if (context == null) throw new InvalidOperationException("No se pudo acceder al DbContext interno del repositorio.");

            return await context.Reservations
                                .Include(r => r.ReservationStatusId)
                                .FirstOrDefaultAsync(r => r.Id == id);
        }
    }
}
