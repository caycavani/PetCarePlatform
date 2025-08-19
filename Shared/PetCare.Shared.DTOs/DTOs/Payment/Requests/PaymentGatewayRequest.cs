namespace PetCare.Shared.DTOs.DTOs.Payment.Requests
{
    public class PaymentGatewayRequest
    {
        public Guid ReservationId { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; } = "COP";
        public string PaymentMethodToken { get; set; } = string.Empty;
        public string CustomerEmail { get; set; } = string.Empty;
    }

}
