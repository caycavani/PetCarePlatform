using PetCare.Booking.Domain.DTOs;
using PetCare.Booking.Domain.Entities;

namespace PetCare.Booking.Extensions
{
    public static class ReservationMappingExtensions
    {
        public static CreateReservationDto ToDto(this Reservation reservation)
        {
            return new CreateReservationDto
            {
                Id = reservation.Id,
                ClientId = reservation.ClientId,
                CaregiverId = reservation.CaregiverId,
                PetId = reservation.PetId,
                StartDate = reservation.StartDate,
                EndDate = reservation.EndDate,
                Note = reservation.Note
            };
        }

        public static Reservation FromDto(CreateReservationDto dto)
        {
            return new Reservation(dto.Id, dto.ClientId, dto.CaregiverId, dto.PetId, dto.StartDate, dto.EndDate,dto.Note)
            {
                Note = dto.Note
            };
        }
    }
}
