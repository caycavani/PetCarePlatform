namespace PetCare.Booking.Application.UseCases
{
    using System;
    using System.Threading.Tasks;
    using PetCare.Booking.Application.DTOs;
    using PetCare.Booking.Domain.Entities;
    using PetCare.Booking.Domain.Interfaces;

    public class CreateReservationUseCase
    {
        private readonly IReservationRepository _reservationRepository;

        public CreateReservationUseCase(IReservationRepository reservationRepository)
        {
            _reservationRepository = reservationRepository;
        }

        public async Task<Guid> ExecuteAsync(CreateReservationDto dto)
        {
            var reservation = new Reservation(
                Guid.NewGuid(),             // id
                dto.ClientId,               // clientId
                dto.CaregiverId,            // caregiverId
                dto.PetId,                  // petId
                dto.StartDate,              // startDate
                dto.EndDate,                // endDate
                dto.Note                    // note
            );

            reservation.UpdateStatus(1); // 1 = Estado pendiente

            var created = await _reservationRepository.CreateAsync(reservation);
            return created ? reservation.Id : Guid.Empty;
        }
    }
}
