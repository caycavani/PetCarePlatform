using System;
using PetCare.Notification.Application.DTOs;
using PetCare.Notification.Application.DTOs.Requests;
using PetCare.Notification.Domain.Entities;
using PetCare.Notification.Domain.Value_Objects;
using PetCare.Notification.Domain.Value_Objects.Enums;

namespace PetCare.Notification.Application.Mappers
{
    /// <summary>
    /// Mapeador entre DTOs y entidades de dominio para notificaciones.
    /// </summary>
    public static class NotificationMapper
    {
        /// <summary>
        /// Convierte un DTO de creación en una entidad Ntification.
        /// </summary>
        /// <param name="dto">DTO con los datos de la notificación.</param>
        /// <returns>Instancia de Ntification lista para procesar.</returns>
        public static Ntification MapFromDto(CreateNotificationDto dto)
        {
            var recipient = new Recipient(
                name: dto.RecipientName,
                email: dto.RecipientEmail,
                phoneNumber: dto.RecipientPhone,
                deviceToken: dto.DeviceToken // ← Obligatorio para canal Push
            );

            var content = new NotificationContent(
                subject: dto.Subject,
                message: dto.Message
            );

            var channel = Enum.Parse<NotificationChannel>(
                dto.Channel,
                ignoreCase: true
            );

            var scheduledAt = dto.ScheduledAt ?? DateTime.UtcNow.AddMinutes(1); // ← Fallback si no se especifica

            return new Ntification(
                id: Guid.NewGuid(),
                recipient: recipient,
                content: content,
                channel: channel,
                scheduledAt: scheduledAt
            );
        }
    }
}
