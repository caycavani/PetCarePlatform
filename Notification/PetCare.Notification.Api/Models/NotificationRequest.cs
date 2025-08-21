using System.ComponentModel.DataAnnotations;

namespace PetCare.Notification.Api.Models
{
    /// <summary>
    /// Modelo de entrada para publicar una notificación.
    /// </summary>
    public class NotificationRequest
    {
        [Required]
        public string Recipient { get; set; } = string.Empty;

        [Required]
        public string Channel { get; set; } = string.Empty;

        [Required]
        [MaxLength(1000)]
        public string Message { get; set; } = string.Empty;
    }
}