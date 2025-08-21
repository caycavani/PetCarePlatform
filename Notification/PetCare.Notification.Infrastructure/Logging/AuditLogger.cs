using Microsoft.Extensions.Logging;
using System;

namespace PetCare.Notification.Infrastructure.Logging
{
    public class AuditLogger
    {
        private readonly ILogger<AuditLogger> _logger;

        public AuditLogger(ILogger<AuditLogger> logger)
        {
            _logger = logger;
        }

        public void LogNotificationSent(Guid notificationId, string recipientEmail, DateTime timestamp)
        {
            _logger.LogInformation("📨 Notification sent | Id: {NotificationId} | Recipient: {Email} | SentAt: {Timestamp}",
                notificationId, recipientEmail, timestamp);
        }

        public void LogError(Guid notificationId, string errorMessage)
        {
            _logger.LogError("❌ Error sending notification | Id: {NotificationId} | Error: {Error}",
                notificationId, errorMessage);
        }

        public void LogEvent(string eventName, object details)
        {
            _logger.LogInformation("📘 Event: {EventName} | Details: {@Details}", eventName, details);
        }
    }
}
