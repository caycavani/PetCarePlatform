using PetCare.Booking.Domain.Entities;

namespace PetCare.Booking.Extensions
{
    public static class ReservationExtensions
    {
        public static bool IsActive(this Reservation reservation)
        {
            return reservation?.Status?.Id == ReservationStatus.Accepted.Id;
        }

        public static bool IsCancelled(this Reservation reservation)
        {
            return reservation?.Status?.Id == ReservationStatus.Cancelled.Id;
        }

        public static bool IsOverlapping(this Reservation reservation, DateTime start, DateTime end)
        {
            if (reservation == null) return false;
            return reservation.StartDate < end && reservation.EndDate > start;
        }

        public static bool IsOwnedBy(this Reservation reservation, Guid clientId)
        {
            return reservation?.ClientId == clientId;
        }

        public static bool IsValidDateRange(this Reservation reservation)
        {
            return reservation != null && reservation.StartDate < reservation.EndDate;
        }
    }
}
