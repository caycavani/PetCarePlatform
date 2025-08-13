using PetCare.Pets.Domain.Entities;

namespace PetCare.Pets.Domain.Interfaces
{
    public interface IPetRepository
    {
        Task<Pet?> GetByIdAsync(Guid id);
        Task<Pet?> GetByIdAndOwnerAsync(Guid petId, Guid ownerId); // ✅ Validación directa
        Task<IEnumerable<Pet>> GetAllAsync();
        Task<Guid> CreateAsync(Pet pet);
        Task UpdateAsync(Pet pet);
        Task DeleteAsync(Guid id);
    }
}
