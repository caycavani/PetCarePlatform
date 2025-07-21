namespace PetCare.Booking.Application.UseCases
{
    using System.Threading.Tasks;
    using PetCare.Booking.Application.DTOs;
    using PetCare.Booking.Domain.Entities;

    public class CreateReservationUseCase
    {
        private readonly IReservationRepository _bookingRepository;

        public CreateReservationUseCase(IReservationRepository bookingRepository)
        {
            _bookingRepository = bookingRepository;
        }

        public async Task<Guid> ExecuteAsync(CreateReservationDto dto)
        {
            var booking = new Reservation(dto.PetId, dto.CaregiverId, dto.StartDate, dto.EndDate);
            await _bookingRepository.AddAsync(booking);
            return booking.Id;
        }
    }
}
