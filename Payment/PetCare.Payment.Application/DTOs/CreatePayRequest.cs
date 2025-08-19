using System.ComponentModel.DataAnnotations;

namespace PetCare.Payment.Application.DTOs
{
    public class CreatePayRequest
    {
        [Required]
        public Guid ReservationId { get; set; }

        [Required]
        [Range(0.01, double.MaxValue)]
        public decimal Amount { get; set; }

        [Required]
        public int PaymentMethodId { get; set; }
    }
}
