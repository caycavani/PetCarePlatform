namespace PetCare.Payment.Application.DTOs
{
    using System.ComponentModel.DataAnnotations;

    public class CreatePaymentDto
    {
        [Required]
        public Guid ReservationId { get; set; }

        [Required]
        [Range(0.01, double.MaxValue)]
        public decimal Amount { get; set; }

        [Required]
        [MaxLength(50)]
        public string Method { get; set; } = string.Empty;
    }
}
