using Microsoft.EntityFrameworkCore;
using PetCare.Pets.Domain.Entities;
using PetCare.Pets.Domain.Interfaces;
using PetCare.Pets.Infrastructure.Persistence;

namespace PetCare.Pets.Infrastructure.Repositories
{
    public class PetRepository : IPetRepository
    {
        private readonly PetDbContext _context;

        public PetRepository(PetDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Pet pet)
        {
            await _context.Pets.AddAsync(pet);
            await _context.SaveChangesAsync();
        }

        public async Task<Pet?> GetByIdAsync(Guid id)
        {
            return await _context.Pets.FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<IEnumerable<Pet>> GetByOwnerAsync(Guid ownerId)
        {
            return await _context.Pets
                .Where(p => p.OwnerId == ownerId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Pet>> GetAllAsync()
        {
            return await _context.Pets.ToListAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var pet = await _context.Pets.FindAsync(id);
            if (pet is null) return;

            _context.Pets.Remove(pet);
            await _context.SaveChangesAsync();
        }
    }
}
