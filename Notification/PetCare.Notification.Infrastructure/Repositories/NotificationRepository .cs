using Microsoft.EntityFrameworkCore;
using PetCare.Notification.Domain.Entities;
using PetCare.Notification.Domain.Interfaces;
using PetCare.Notification.Infrastructure.Persistence;

namespace PetCare.Notification.Infrastructure.Repositories
{
    public class NotificationRepository : INotificationRepository
    {
        private readonly NotificationDbContext _context;

        public NotificationRepository(NotificationDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(RNotification notification)
        {
            await _context.Notifications.AddAsync(notification);
            await _context.SaveChangesAsync();
        }

        public async Task<RNotification?> GetByIdAsync(Guid id)
        {
            return await _context.Notifications.FirstOrDefaultAsync(n => n.Id == id);
        }

        public async Task<IEnumerable<RNotification>> GetByRecipientAsync(Guid recipientId)
        {
            return await _context.Notifications
                .Where(n => n.RecipientId == recipientId)
                .ToListAsync();
        }

        public async Task<IEnumerable<RNotification>> GetAllAsync()
        {
            return await _context.Notifications.ToListAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var notification = await _context.Notifications.FindAsync(id);
            if (notification is null) return;

            _context.Notifications.Remove(notification);
            await _context.SaveChangesAsync();
        }
    }
}
