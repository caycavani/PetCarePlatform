using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PetCare.Booking.Domain.Entities;

public interface IReservationRepository
{
    Task AddAsync(Reservation reservation);
    Task<Reservation?> GetByIdAsync(Guid id);
    Task<IEnumerable<Reservation>> GetByCaregiverAsync(Guid caregiverId);
    Task<IEnumerable<Reservation>> GetByPetAsync(Guid petId);
    Task UpdateAsync(Reservation reservation);
    Task DeleteAsync(Guid reservationId);
}
