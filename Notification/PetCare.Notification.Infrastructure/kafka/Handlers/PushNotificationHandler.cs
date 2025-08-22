using PetCare.Notification.Application.DTOs.kafka;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetCare.Notification.Infrastructure.kafka.Handlers
{
    public class PushNotificationHandler
    {
        public Task HandleAsync(NotificationEventDto dto)
        {
            // Simulación de envío de notificación push
            Console.WriteLine($"[PUSH] To: {dto.Recipient} | Title: {dto.Subject} | Message: {dto.Message}");
            return Task.CompletedTask;
        }
    }
}
