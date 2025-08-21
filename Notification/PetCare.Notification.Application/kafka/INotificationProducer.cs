using PetCare.Notification.Domain.Events;

namespace PetCare.Notification.Application.kafka
{
    public interface INotificationProducer
    {
        Task PublishAsync(NotificationCreated notification);
    }
}
