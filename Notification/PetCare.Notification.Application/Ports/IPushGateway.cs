namespace PetCare.Notification.Application.Ports
{
    /// <summary>
    /// Puerto de salida para el envío de notificaciones push.
    /// </summary>
    public interface IPushGateway
    {
        Task<bool> SendPushAsync(string deviceToken, string title, string body);
    }
}
