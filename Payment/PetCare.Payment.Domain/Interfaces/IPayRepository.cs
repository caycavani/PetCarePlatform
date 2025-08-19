namespace PetCare.Payment.Domain.Repositories
{
    using System;
    using System.Threading.Tasks;
    using PetCare.Payment.Domain.Entities;

    public interface IPayRepository
    {
        Task<Pay?> GetByIdAsync(Guid id);
        Task AddAsync(Pay pay);
        Task SaveChangesAsync();
    }
}
