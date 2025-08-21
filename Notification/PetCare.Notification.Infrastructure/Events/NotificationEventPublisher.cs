using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using PetCare.Notification.Domain.Entities;

namespace PetCare.Notification.Infrastructure.Events
{
    public class NotificationEventPublisher
    {
        private readonly ILogger<NotificationEventPublisher> _logger;

        public NotificationEventPublisher(ILogger<NotificationEventPublisher> logger)
        {
            _logger = logger;
        }

        public async Task PublishNotificationSentAsync(Ntification notification)
        {
            // Simulación de publicación (puede integrarse con RabbitMQ, Kafka, etc.)
            await Task.Delay(50); // Simula latencia

            _logger.LogInformation("📤 Published NotificationSent event | Id: {Id} | Channel: {Channel} | Recipient: {Email}",
                notification.Id, notification.Channel, notification.Recipient.Email);
        }
    }
}
