using Microsoft.EntityFrameworkCore;
using PetCare.Booking.Domain.Entities;
using PetCare.Booking.Domain.Interfaces;
using PetCare.Booking.Infrastructure.Persistence;
using System;
using System.Threading.Tasks;

namespace PetCare.Booking.Infrastructure.Repositories
{
    public class ServiceRepository : IServiceRepository
    {
        private readonly ReservationDbContext _context;

        public ServiceRepository(ReservationDbContext context)
        {
            _context = context;
        }

        public async Task<Service?> GetByIdAsync(Guid id)
            => await _context.Services.FindAsync(id);

        public async Task<IEnumerable<Service>> GetAllAsync()
            => await _context.Services.ToListAsync();

        public async Task<bool> CreateAsync(Service service)
        {
            _context.Services.Add(service);
            return await _context.SaveChangesAsync() > 0;
        }
    }

}
