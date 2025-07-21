namespace PetCare.Review.Infrastructure.Repositories
{
    using Microsoft.EntityFrameworkCore;
    using PetCare.Review.Domain.Entities;
    using PetCare.Review.Domain.Interfaces;
    using PetCare.Review.Infrastructure.Persistence;

    public class ReviewRepository : IReviewRepository
    {
        private readonly ReviewDbContext _context;

        public ReviewRepository(ReviewDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Rview review)
        {
            await _context.Reviews.AddAsync(review);
            await _context.SaveChangesAsync();
        }

        public async Task<Rview?> GetByIdAsync(Guid id)
        {
            return await _context.Reviews.FirstOrDefaultAsync(r => r.Id == id);
        }

        public async Task<IEnumerable<Rview>> GetByReservationAsync(Guid reservationId)
        {
            return await _context.Reviews
                .Where(r => r.ReservationId == reservationId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Rview>> GetAllAsync()
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
    }
}
