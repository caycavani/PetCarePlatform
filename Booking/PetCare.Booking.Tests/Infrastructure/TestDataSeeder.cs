using System;
using System.Threading.Tasks;
using PetCare.Booking.Domain.Entities;
using PetCare.Booking.Infrastructure.Persistence;
using PetCare.Booking.Tests.Seeding;

namespace PetCare.Booking.Tests.Infrastructure;

public static class TestDataSeeder
{
    public static async Task SeedNoteOnlyReservationAsync(
        IServiceProvider services,
        Guid clientId,
        Guid reservationId,
        string note)
    {
        if (string.IsNullOrWhiteSpace(note) || note.Length < 1 || note.Length > 500)
        {
            throw new ArgumentException("La nota debe tener entre 1 y 500 caracteres.");
        }

        var reservation = new Reservation(
      reservationId,
      clientId,
      Guid.NewGuid(),              // CaregiverId
      Guid.NewGuid(),              // PetId
      DateTime.UtcNow.Date,
      DateTime.UtcNow.Date.AddDays(2),
      note
  );

        reservation.ReservationStatusId = 1;
        reservation.CreatedAt = DateTime.UtcNow;

    }

    public static async Task SeedReservationWithCustomDatesAndNoteAsync(
        IServiceProvider services,
        Guid clientId,
        Guid reservationId,
        DateTime startDate,
        DateTime endDate,
        string? note,
        int statusId = 1) // STATUS_PENDING
    {
        if (endDate <= startDate)
            throw new ArgumentException("La fecha de fin debe ser posterior a la de inicio.");

        if (!string.IsNullOrWhiteSpace(note) && (note.Length < 1 || note.Length > 500))
            throw new ArgumentException("La nota debe tener entre 1 y 500 caracteres si se incluye.");

        var reservation = new Reservation(
       reservationId,
       clientId,
       Guid.NewGuid(),            // CaregiverId
       Guid.NewGuid(),            // PetId
       startDate,
       endDate,
       note
   );

        reservation.ReservationStatusId = statusId;
        reservation.CreatedAt = DateTime.UtcNow;

        await ReservationSeeder.SeedAsync(services, reservation);


    }
}
