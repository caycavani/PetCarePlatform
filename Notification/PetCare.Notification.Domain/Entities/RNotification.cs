namespace PetCare.Notification.Domain.Entities
{
    using System;

    public class RNotification
    {
        public Guid Id { get; private set; }
        public Guid RecipientId { get; private set; }
        public string Message { get; private set; }
        public DateTime SentAt { get; private set; }

        protected RNotification() { }

        public RNotification(Guid recipientId, string message)
        {
            Id = Guid.NewGuid();
            RecipientId = recipientId;
            Message = message;
            SentAt = DateTime.UtcNow;
        }
    }
}
