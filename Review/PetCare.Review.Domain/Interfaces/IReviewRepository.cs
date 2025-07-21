namespace PetCare.Review.Domain.Interfaces
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using PetCare.Review.Domain.Entities;

    public interface IReviewRepository
    {
        Task AddAsync(Rview review);

        Task<Rview?> GetByIdAsync(Guid id);

        Task<IEnumerable<Rview>> GetByReservationAsync(Guid reservationId);

        Task<IEnumerable<Rview>> GetAllAsync();

        Task DeleteAsync(Guid id);
    }
}
