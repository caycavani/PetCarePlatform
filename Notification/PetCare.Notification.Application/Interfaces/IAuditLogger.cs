namespace PetCare.Notification.Application.Interfaces
{
    public interface IAuditLogger
    {
        Task LogAsync(string action, object data);
    }

}
