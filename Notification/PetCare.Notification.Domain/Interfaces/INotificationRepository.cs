namespace PetCare.Notification.Domain.Interfaces
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using PetCare.Notification.Domain.Entities;

    public interface INotificationRepository
    {
        Task AddAsync(RNotification notification);

        Task<RNotification?> GetByIdAsync(Guid id);

        Task<IEnumerable<RNotification>> GetByRecipientAsync(Guid recipientId);

        Task<IEnumerable<RNotification>> GetAllAsync();

        Task DeleteAsync(Guid id);
    }
}
