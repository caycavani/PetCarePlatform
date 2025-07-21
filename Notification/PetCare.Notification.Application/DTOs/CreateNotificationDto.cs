namespace PetCare.Notification.Application.DTOs
{
    using System.ComponentModel.DataAnnotations;

    public class CreateNotificationDto
    {
        [Required]
        public Guid RecipientId { get; set; }

        [Required]
        [MaxLength(500)]
        public string Message { get; set; } = string.Empty;
    }
}
