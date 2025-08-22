namespace PetCare.Notification.Application.DTOs.kafka
{
    public class NotificationEventDto
    {
        public Guid Id { get; set; }
        public string Channel { get; set; } = default!;
        public string Recipient { get; set; } = default!;
        public string Subject { get; set; } = default!;
        public string Message { get; set; } = default!;
        public DateTime Timestamp { get; set; }
    }
}
