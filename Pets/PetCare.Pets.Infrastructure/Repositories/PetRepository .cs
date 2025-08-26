using Microsoft.EntityFrameworkCore;
using PetCare.Pets.Domain.Entities;
using PetCare.Pets.Domain.Interfaces;
using PetCare.Pets.Infrastructure.Persistence;

namespace PetCare.Pets.Infrastructure.Repositories
{
    /// <summary>
    /// Implementación de acceso a datos para mascotas usando Entity Framework Core.
    /// </summary>
    public class PetRepository : IPetRepository
    {
        private readonly PetDbContext _context;

        public PetRepository(PetDbContext context)
        {
            _context = context;
        }

        /// <inheritdoc />
        public async Task<Guid> CreateAsync(Pet pet)
        {
            await _context.Pets.AddAsync(pet);
            await _context.SaveChangesAsync();
            return pet.Id;
        }

        /// <inheritdoc />
        public async Task UpdateAsync(Pet pet)
        {
            var existingPet = await _context.Pets.FindAsync(pet.Id);
            if (existingPet is null) return;

            existingPet.Name = pet.Name;
            existingPet.Breed = pet.Breed;
            existingPet.Age = pet.Age;
            existingPet.Type = pet.Type;
            existingPet.BirthDate = pet.BirthDate;
            existingPet.OwnerId = pet.OwnerId;

            _context.Pets.Update(existingPet);
            await _context.SaveChangesAsync();
        }

        /// <inheritdoc />
        public async Task<Pet?> GetByIdAsync(Guid id)
        {
            return await _context.Pets
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        /// <inheritdoc />
        public async Task<Pet?> GetByIdAndOwnerAsync(Guid petId, Guid ownerId)
        {
            return await _context.Pets
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == petId && p.OwnerId == ownerId);
        }

        /// <inheritdoc />
        public async Task<IEnumerable<Pet>> GetByOwnerIdAsync(Guid ownerId)
        {
            return await _context.Pets
                .AsNoTracking()
                .Where(p => p.OwnerId == ownerId)
                .ToListAsync();
        }

        /// <inheritdoc />
        public async Task<IEnumerable<Pet>> GetAllAsync()
        {
            return await _context.Pets
                .AsNoTracking()
                .ToListAsync();
        }

        /// <inheritdoc />
        public async Task DeleteAsync(Guid id)
        {
            var pet = await _context.Pets.FindAsync(id);
            if (pet is null) return;

            _context.Pets.Remove(pet);
            await _context.SaveChangesAsync();
        }
    }
}
