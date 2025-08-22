using PetCare.Notification.Application.DTOs.kafka;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetCare.Notification.Infrastructure.kafka.Handlers
{
    public class SmsNotificationHandler
    {
        public Task HandleAsync(NotificationEventDto dto)
        {
            // Simulación de envío de SMS
            Console.WriteLine($"[SMS] To: {dto.Recipient} | Message: {dto.Message}");
            return Task.CompletedTask;
        }
    }
}
