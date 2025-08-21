namespace PetCare.Notification.Domain.Events
{
    using System;

    public class NotificationCreated
    {
        public Guid Id { get; set; }
        public string Message { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
