using Microsoft.EntityFrameworkCore;
using PetCare.Pets.Domain.Entities;
using PetCare.Pets.Infrastructure.Persistence;
using PetCare.Pets.Domain.Interfaces; // Asegúrate de que la interfaz esté accesible

namespace PetCare.Pets.Infrastructure.Repositories
{
    public class PetRepository : IPetRepository
    {
        private readonly PetDbContext _context;

        public PetRepository(PetDbContext context)
        {
            _context = context;
        }

        public async Task<Guid> CreateAsync(Pet pet)
        {
            await _context.Pets.AddAsync(pet);
            await _context.SaveChangesAsync();
            return pet.Id;
        }

        public async Task UpdateAsync(Pet pet)
        {
            var existingPet = await _context.Pets.FindAsync(pet.Id);
            if (existingPet is null) return;

            existingPet.Name = pet.Name;
            existingPet.Breed = pet.Breed;
            existingPet.Age = pet.Age;
            existingPet.OwnerId = pet.OwnerId;
            // Agrega más campos si es necesario

            _context.Pets.Update(existingPet);
            await _context.SaveChangesAsync();
        }

        public async Task<Pet?> GetByIdAsync(Guid id)
        {
            return await _context.Pets.FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<Pet?> GetByIdAndOwnerAsync(Guid petId, Guid ownerId)
        {
            return await _context.Pets
                .FirstOrDefaultAsync(p => p.Id == petId && p.OwnerId == ownerId);
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
