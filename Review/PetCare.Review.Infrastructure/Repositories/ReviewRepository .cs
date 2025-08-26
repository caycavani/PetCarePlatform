namespace PetCare.Review.Infrastructure.Repositories
{
    using Microsoft.EntityFrameworkCore;
    using PetCare.Review.Domain.Entities;
    using PetCare.Review.Domain.Interfaces;
    using PetCare.Review.Infrastructure.Persistence;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public class ReviewRepository : IReviewRepository
    {
        private readonly ReviewDbContext _context;

        public ReviewRepository(ReviewDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Review review)
        {
            await _context.Reviews.AddAsync(review);
            await _context.SaveChangesAsync();
        }

        public async Task<Review?> GetByIdAsync(Guid id)
        {
            return await _context.Reviews
                .FirstOrDefaultAsync(r => r.Id == id);
        }

        public async Task<IEnumerable<Review>> GetByReservationAsync(Guid reservationId)
        {
            return await _context.Reviews
                .Where(r => r.ReservationId == reservationId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Review>> GetAllAsync()
        {
            return await _context.Reviews.ToListAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var review = await _context.Reviews.FindAsync(id);
            if (review is null) return;

            _context.Reviews.Remove(review);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> ExistsByReservationAsync(Guid reservationId)
        {
            return await _context.Reviews
                .AnyAsync(r => r.ReservationId == reservationId);
        }
    }
}
