namespace PetCare.Notification.Application.Ports
{
    public interface ISmsGateway
    {
        Task<bool> SendSmsAsync(string phoneNumber, string message);
    }
}
