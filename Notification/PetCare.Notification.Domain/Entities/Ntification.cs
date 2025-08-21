using System;
using PetCare.Notification.Domain.Exceptions;
using PetCare.Notification.Domain.Value_Objects;
using PetCare.Notification.Domain.Value_Objects.Enums;

namespace PetCare.Notification.Domain.Entities
{
    /// <summary>
    /// Entidad que representa una notificación programada para ser enviada a un destinatario por un canal específico.
    /// </summary>
    public class Ntification
    {
        public Guid Id { get; private set; }
        public Recipient Recipient { get; private set; }
        public NotificationContent Content { get; private set; }
        public NotificationChannel Channel { get; private set; }
        public DateTime ScheduledAt { get; private set; }

        /// <summary>
        /// Constructor principal que inicializa una notificación con validaciones semánticas.
        /// </summary>
        public Ntification(Guid id, Recipient recipient, NotificationContent content, NotificationChannel channel, DateTime scheduledAt)
        {
            ValidateId(id);
            ValidateRecipient(recipient, channel);
            ValidateContent(content);
            ValidateChannel(channel);
            ValidateScheduledAt(scheduledAt);

            Id = id;
            Recipient = recipient;
            Content = content;
            Channel = channel;
            ScheduledAt = scheduledAt;
        }

        /// <summary>
        /// Constructor protegido requerido por EF Core para instanciación interna.
        /// </summary>
        protected Ntification() { }

        /// <summary>
        /// Determina si la notificación está lista para ser enviada según la hora actual.
        /// </summary>
        public bool IsReadyToSend => DateTime.UtcNow >= ScheduledAt;

        /// <summary>
        /// Representación semántica para trazabilidad y logging.
        /// </summary>
        public override string ToString()
        {
            return $"[Ntification] Id={Id}, Channel={Channel}, Recipient={Recipient?.Email}, ScheduledAt={ScheduledAt:u}";
        }

        #region Validaciones privadas

        private void ValidateId(Guid id)
        {
            if (id == Guid.Empty)
                throw new InvalidNotificationException("El identificador de la notificación no puede estar vacío.");
        }

        private void ValidateRecipient(Recipient recipient, NotificationChannel channel)
        {
            if (recipient is null)
                throw new InvalidNotificationException("El destinatario no puede ser nulo.");

            if (string.IsNullOrWhiteSpace(recipient.Name))
                throw new InvalidNotificationException("El nombre del destinatario no puede estar vacío.");

            if (string.IsNullOrWhiteSpace(recipient.Email))
                throw new InvalidNotificationException("El correo electrónico del destinatario no puede estar vacío.");

            if (channel == NotificationChannel.Push && string.IsNullOrWhiteSpace(recipient.DeviceToken))
                throw new InvalidNotificationException("Las notificaciones Push requieren un deviceToken válido.");
        }

        private void ValidateContent(NotificationContent content)
        {
            if (content is null)
                throw new InvalidNotificationException("El contenido de la notificación no puede ser nulo.");

            if (string.IsNullOrWhiteSpace(content.Message))
                throw new InvalidNotificationException("El mensaje de la notificación no puede estar vacío.");
        }

        private void ValidateChannel(NotificationChannel channel)
        {
            if (!Enum.IsDefined(typeof(NotificationChannel), channel))
                throw new InvalidNotificationException("El canal de notificación especificado no es válido.");
        }

        private void ValidateScheduledAt(DateTime scheduledAt)
        {
            if (scheduledAt < DateTime.UtcNow.AddSeconds(-5))
                throw new InvalidNotificationException("La fecha de programación no puede estar en el pasado.");
        }

        #endregion
    }
}
