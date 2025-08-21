namespace PetCare.Notification.Application.Interfaces
{
    using PetCare.Notification.Domain.Entities;

    public interface INotificationRepository
    {
        Task SaveAsync(Ntification notification);
        Task<Ntification?> GetByIdAsync(Guid id);
    }

}
