namespace PetCare.Notification.Application.DTOs.Requests
{
    using System.ComponentModel.DataAnnotations;

    /// <summary>
    /// DTO para la creación de notificaciones.
    /// </summary>
    public class CreateNotificationDto
    {
        [Required]
        public string RecipientName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string RecipientEmail { get; set; } = string.Empty;

        [Required]
        [Phone]
        public string RecipientPhone { get; set; } = string.Empty;

        /// <summary>
        /// Token del dispositivo para notificaciones push.
        /// </summary>
        public string? DeviceToken { get; set; }

        [Required]
        public string Subject { get; set; } = string.Empty;

        [Required]
        public string Message { get; set; } = string.Empty;

        [Required]
        [RegularExpression("Email|SMS|Push", ErrorMessage = "El canal debe ser Email, SMS o Push.")]
        public string Channel { get; set; } = string.Empty;

        /// <summary>
        /// Fecha programada para el envío de la notificación.
        /// </summary>
        public DateTime? ScheduledAt { get; set; }
    }
}
