using PetCare.Booking.Domain.Entities;
using System;
using System.Threading.Tasks;

namespace PetCare.Booking.Domain.Interfaces
{
    public interface IServiceRepository
    {
        Task<Service?> GetByIdAsync(Guid id);
        Task<IEnumerable<Service>> GetAllAsync();
        Task<bool> CreateAsync(Service service);
    }
}
