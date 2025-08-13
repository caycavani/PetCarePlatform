namespace PetCare.Booking.Tests.Reservations
{
    using Microsoft.EntityFrameworkCore;
    using NUnit.Framework;
    using PetCare.Booking.Domain.Entities;
    using PetCare.Booking.Infrastructure.Persistence;
    using PetCare.Booking.Tests.Builders;
    using System.Linq;

    public class ReservationTests
    {
        private ReservationDbContext _context;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<ReservationDbContext>()
                .UseInMemoryDatabase("TestDb")
                .Options;

            _context = new ReservationDbContext(options);
        }

        [Test]
        public void ShouldCreateReservationWithConfirmedStatus()
        {
            var reservation = new ReservationBuilder()
                .WithStatus(2) // Confirmed
                .Build();

            _context.Reservations.Add(reservation);
            _context.SaveChanges();

            var result = _context.Reservations.First();

            Assert.Equals(2, result.ReservationStatusId); // Ajuste en método de aserción
        }
    }
}
