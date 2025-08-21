namespace PetCare.Notification.Application.DTOs
{
    public class NotificationResultDto
    {
        public Guid NotificationId { get; set; }
        public bool Success { get; set; }
        public string? ErrorMessage { get; set; }
    }

}
