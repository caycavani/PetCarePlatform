using PetCare.Notification.Application.DTOs.kafka;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetCare.Notification.Infrastructure.kafka.Handlers
{
    public class EmailNotificationHandler
    {
        public Task HandleAsync(NotificationEventDto dto)
        {
            // Simulación de envío de correo
            Console.WriteLine($"[EMAIL] To: {dto.Recipient} | Subject: {dto.Subject} | Body: {dto.Message}");
            return Task.CompletedTask;
        }
    }
}
