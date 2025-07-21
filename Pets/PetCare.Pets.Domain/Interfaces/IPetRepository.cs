using PetCare.Pets.Domain.Entities;

namespace PetCare.Pets.Domain.Interfaces
{
    public interface IPetRepository
    {
        Task AddAsync(Pet pet);
        Task<Pet?> GetByIdAsync(Guid id);
        Task<IEnumerable<Pet>> GetByOwnerAsync(Guid ownerId);
        Task<IEnumerable<Pet>> GetAllAsync();
        Task DeleteAsync(Guid id);
    }
}
