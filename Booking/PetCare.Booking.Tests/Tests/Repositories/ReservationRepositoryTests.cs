using Microsoft.EntityFrameworkCore;
using PetCare.Booking.Domain.Entities;
using PetCare.Booking.Infrastructure.Persistence;
using PetCare.Booking.Infrastructure.Repositories;
using System;
using System.Threading.Tasks;
using Xunit;

public class ReservationRepositoryTests
{
    [Fact]
    public async Task GetByIdAsync_ReservaExistente_RetornaReserva()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<ReservationDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDb_ReservaExistente")
            .Options;

        var reservaId = Guid.NewGuid();
        var clienteId = Guid.NewGuid();
        var caregiverId = Guid.NewGuid();
        var petId = Guid.NewGuid();

        using (var context = new ReservationDbContext(options))
        {
            var reservation = new Reservation(
                reservaId,
                clienteId,
                caregiverId,
                petId,
                DateTime.Today,
                DateTime.Today.AddDays(2),
                "Test note"
            );

            reservation.ReservationStatusId = 1;

            context.Reservations.Add(reservation);
            await context.SaveChangesAsync();
        }

        // Act
        using (var context = new ReservationDbContext(options))
        {
            var repository = new ReservationRepository(context);
            var result = await repository.GetByIdAsync(reservaId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(reservaId, result!.Id);
            Assert.Equal("Test note", result.Note);
        }
    }

    [Fact]
    public async Task GetByIdAsync_ReservaInexistente_RetornaNull()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<ReservationDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDb_ReservaInexistente")
            .Options;

        using (var context = new ReservationDbContext(options))
        {
            var repository = new ReservationRepository(context);

            // Act
            var result = await repository.GetByIdAsync(Guid.NewGuid());

            // Assert
            Assert.Null(result);
        }
    }
}
