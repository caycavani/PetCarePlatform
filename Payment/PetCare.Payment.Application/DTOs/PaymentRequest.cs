
namespace PetCare.Payment.Application.DTOs
{
    public class PaymentRequest
    {
        public Guid ReservationId { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; }
        public string PaymentMethodToken { get; set; }
        public string CustomerEmail { get; set; }
    }

}
