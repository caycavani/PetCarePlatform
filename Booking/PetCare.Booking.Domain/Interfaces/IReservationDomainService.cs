using PetCare.Booking.Domain.Entities;

namespace PetCare.Booking.Domain.Interfaces
{
    public interface IReservationDomainService
    {
        Task<Reservation?> GetByIdAsync(Guid id);
        Task<IEnumerable<Reservation>> GetAllAsync();
        Task CreateAsync(Reservation reservation);
        Task<bool> CancelAsync(Guid id);
        Task<bool> AcceptAsync(Guid id);
        Task<bool> UpdateNoteAsync(Guid id, string note);
    }
}
