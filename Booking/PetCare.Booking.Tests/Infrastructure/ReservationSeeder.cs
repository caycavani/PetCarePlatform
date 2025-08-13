using Microsoft.Extensions.DependencyInjection;
using PetCare.Booking.Domain.Entities;
using PetCare.Booking.Infrastructure.Persistence;
using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace PetCare.Booking.Tests.Seeding
{
    public static class ReservationSeeder
    {
        public static async Task SeedAsync(IServiceProvider services, Reservation reservation)
        {
            using var scope = services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ReservationDbContext>();

            // 🧼 Validar si la reserva ya existe
            var existing = await context.Reservations
                .AsNoTracking()
                .FirstOrDefaultAsync(r => r.Id == reservation.Id);

            if (existing != null)
            {
                context.Reservations.Remove(reservation); // opcional: eliminar si se requiere reiniciar estado
                await context.SaveChangesAsync();
            }

            // 📦 Sembrar nueva reserva
            await context.Reservations.AddAsync(reservation);
            await context.SaveChangesAsync(); // ✅ Persistir en BD física
        }

        // 🛠 Extra: método para limpiar tabla de reservas (si lo necesitas entre tests)
        public static async Task ClearAsync(IServiceProvider services)
        {
            using var scope = services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ReservationDbContext>();

            context.Reservations.RemoveRange(context.Reservations);
            await context.SaveChangesAsync();
        }
    }
}
