namespace PetCare.Notification.Application.DTOs.Events
{
    public class NotificationSentEvent
    {
        public Guid NotificationId { get; set; }
        public string Channel { get; set; }
        public DateTime SentAt { get; set; }
        public string Recipient { get; set; }
    }

}
