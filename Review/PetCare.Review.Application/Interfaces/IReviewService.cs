namespace PetCare.Review.Application.Interfaces
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using PetCare.Review.Domain.Entities;

    public interface IReviewService
    {
        Task<Review> CreateAsync(Review review);
        Task<Review?> GetByIdAsync(Guid reviewId);
        Task<IEnumerable<Review>> GetByReservationAsync(Guid reservationId);
        Task<IEnumerable<Review>> GetAllAsync();
        Task DeleteAsync(Guid reviewId);
    }
}
