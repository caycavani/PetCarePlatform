namespace PetCare.Notification.Domain.Interfaces
{
    using System.Threading.Tasks;

    /// <summary>
    /// Define el contrato para publicar eventos de notificación.
    /// </summary>
    public interface INotificationProducer
    {
        Task PublishAsync(string message);
    }
}
