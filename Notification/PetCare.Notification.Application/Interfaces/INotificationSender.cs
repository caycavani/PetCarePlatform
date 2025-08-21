namespace PetCare.Notification.Application.Interfaces
{
    using PetCare.Notification.Domain.Entities;

    public interface INotificationSender
    {
        Task<bool> SendAsync(Ntification notification);
    }

}
