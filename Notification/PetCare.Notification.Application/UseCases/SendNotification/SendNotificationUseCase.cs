using System;
using System.Threading.Tasks;
using PetCare.Notification.Application.DTOs;
using PetCare.Notification.Application.DTOs.Requests;
using PetCare.Notification.Domain.Entities;
using PetCare.Notification.Domain.Interfaces;
using PetCare.Notification.Domain.Value_Objects;
using PetCare.Notification.Domain.Value_Objects.Enums;

namespace PetCare.Notification.Application.UseCases.SendNotification
{
    /// <summary>
    /// Caso de uso para enviar una notificación a través del canal especificado.
    /// </summary>
    public class SendNotificationUseCase
    {
        private readonly INotificationSender _sender;

        public SendNotificationUseCase(INotificationSender sender)
        {
            _sender = sender;
        }

        /// <summary>
        /// Ejecuta el envío de una notificación basada en los datos recibidos.
        /// </summary>
        /// <param name="dto">Datos de entrada para crear la notificación.</param>
        /// <returns>Resultado del envío con identificador y estado.</returns>
        public async Task<NotificationResultDto> ExecuteAsync(CreateNotificationDto dto)
        {
            var recipient = new Recipient(
                name: dto.RecipientName,
                email: dto.RecipientEmail,
                phoneNumber: dto.RecipientPhone,
                deviceToken: dto.DeviceToken
            );

            var content = new NotificationContent(
                subject: dto.Subject,
                message: dto.Message
            );

            var channel = Enum.Parse<NotificationChannel>(
                dto.Channel,
                ignoreCase: true
            );

            var scheduledAt = dto.ScheduledAt ?? DateTime.UtcNow.AddSeconds(10); // Fallback reproducible

            var notification = new Ntification(
                id: Guid.NewGuid(),
                recipient: recipient,
                content: content,
                channel: channel,
                scheduledAt: scheduledAt
            );

            var success = await _sender.SendAsync(notification);

            return new NotificationResultDto
            {
                NotificationId = notification.Id,
                Success = success,
                ErrorMessage = success ? null : "Error al enviar la notificación"
            };
        }
    }
}
