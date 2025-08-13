

using PetCare.Shared.DTOs;

namespace PetCare.Booking.Infrastructure.ExternalServices.Pets.Interfaces
{
    public interface IPetServiceClient
    {
        Task<PetDto?> GetByIdAsync(Guid petId);
        Task<IEnumerable<PetDto>> GetAllAsync();
        Task<PetDto?> GetByIdAndOwnerAsync(Guid petId, Guid ownerId);
    }
}
