using Microsoft.EntityFrameworkCore;
using PetCare.Payment.Domain.Entities;
using PetCare.Payment.Domain.Interfaces;
using PetCare.Payment.Infrastructure.Persistence;

namespace PetCare.Payment.Infrastructure.Repositories
{
    public class PaymentRepository : IPaymentRepository
    {
        private readonly PaymentDbContext _context;

        public PaymentRepository(PaymentDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Pay payment)
        {
            await _context.Payments.AddAsync(payment);
            await _context.SaveChangesAsync();
        }

        public async Task<Pay?> GetByIdAsync(Guid id)
        {
            return await _context.Payments.FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<IEnumerable<Pay>> GetByReservationAsync(Guid reservationId)
        {
            return await _context.Payments
                .Where(p => p.ReservationId == reservationId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Pay>> GetAllAsync()
        {
            return await _context.Payments.ToListAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var payment = await _context.Payments.FindAsync(id);
            if (payment is null) return;

            _context.Payments.Remove(payment);
            await _context.SaveChangesAsync();
        }
    }
}
