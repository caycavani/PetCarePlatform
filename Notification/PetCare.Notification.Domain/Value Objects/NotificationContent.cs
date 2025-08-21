using System;

namespace PetCare.Notification.Domain.Value_Objects
{
    /// <summary>
    /// Value Object que encapsula el contenido textual de una notificación.
    /// </summary>
    public class NotificationContent
    {
        public string Subject { get; }
        public string Message { get; }

        public NotificationContent(string subject, string message)
        {
            Subject = subject ?? throw new ArgumentNullException(nameof(subject));
            Message = message ?? throw new ArgumentNullException(nameof(message));
        }
    }
}
