using Microsoft.EntityFrameworkCore;
using PetCare.Booking.Domain.Entities;
using PetCare.Booking.Infrastructure.Persistence;

namespace PetCare.Booking.Infrastructure.Repositories
{
    public class ReservationRepository : IReservationRepository
    {
        private readonly ReservationDbContext _context;

        public ReservationRepository(ReservationDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Reservation reservation)
        {
            await _context.Reservations.AddAsync(reservation);
            await _context.SaveChangesAsync();
        }

        public async Task<Reservation?> GetByIdAsync(Guid id)
        {
            return await _context.Reservations.FirstOrDefaultAsync(r => r.Id == id);
        }

        public async Task<IEnumerable<Reservation>> GetByCaregiverAsync(Guid caregiverId)
        {
            return await _context.Reservations
                .Where(r => r.CaregiverId == caregiverId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Reservation>> GetByPetAsync(Guid petId)
        {
            return await _context.Reservations
                .Where(r => r.PetId == petId)
                .ToListAsync();
        }

        public async Task UpdateAsync(Reservation reservation)
        {
            _context.Reservations.Update(reservation);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid reservationId)
        {
            var reservation = await _context.Reservations.FindAsync(reservationId);
            if (reservation is null) return;

            _context.Reservations.Remove(reservation);
            await _context.SaveChangesAsync();
        }
    }
}
