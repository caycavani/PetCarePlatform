using Microsoft.Extensions.DependencyInjection;
using PetCare.Booking.Domain.Entities;
using PetCare.Booking.Domain.Interfaces;
using PetCare.Booking.Tests.Builders;
using PetCare.Booking.Tests.Infrastructure;
using PetCare.Booking.Tests.Seeding;
using Xunit;

namespace PetCare.Booking.Tests.Integration
{
    public class ReservationSeedingVerificationTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly CustomWebApplicationFactory _factory;
        private readonly Reservation _reservation;

        public ReservationSeedingVerificationTests(CustomWebApplicationFactory factory)
        {
            _factory = factory;

            // 🔧 Crear la reserva con valores predecibles
            _reservation = new ReservationBuilder()
                .WithStatus(2) // STATUS_ACCEPTED
                .WithDates(DateTime.Today.AddDays(1), DateTime.Today.AddDays(2))
                .WithNote("Reserva para verificación")
                .Build();

            // 🌱 Ejecutar el seeding con la reserva generada
            ReservationSeeder.SeedAsync(_factory.Services, _reservation).GetAwaiter().GetResult();
        }

        [Fact]
        public async Task ReservaSembrada_DeberiaEstarAccesiblePorRepositorio()
        {
            using var scope = _factory.Services.CreateScope();
            var repo = scope.ServiceProvider.GetRequiredService<IReservationRepository>();

            var reserva = await repo.GetByIdAsync(_reservation.Id);

            Assert.NotNull(reserva);
            Assert.Equal(_reservation.ClientId, reserva!.ClientId);
            Assert.Equal(_reservation.ReservationStatusId, reserva.ReservationStatusId);
            Assert.Equal(_reservation.StartDate.Date, reserva.StartDate.Date);
            Assert.Equal(_reservation.EndDate.Date, reserva.EndDate.Date);
            Assert.Equal(_reservation.Note, reserva.Note);
        }
    }
}
