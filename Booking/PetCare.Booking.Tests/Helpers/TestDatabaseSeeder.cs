using Microsoft.Extensions.DependencyInjection;
using PetCare.Booking.Domain.Entities;
using PetCare.Booking.Infrastructure.Persistence;
using PetCare.Booking.Tests.Builders;
using PetCare.Booking.Tests.Infrastructure;
using PetCare.Booking.Tests.Seeding;

namespace PetCare.Booking.Tests.Helpers
{
    public static class TestDatabaseSeeder
    {
        // 🔧 TestDatabaseSeeder.cs
        public static void SeedReservation(IServiceProvider services)
        {
            var reservation = new ReservationBuilder()
                .WithStatus(2)
                .WithNote("Reserva inicial desde seeder")
                .Build();

            using var scope = services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<ReservationDbContext>();

            db.Database.EnsureCreated();
            ReservationSeeder.SeedAsync(scope.ServiceProvider, reservation).GetAwaiter().GetResult();
        }

    }
}
