namespace PetCare.Shared.DTOs.DTOs.Payment.Responses
{
    public class PaymentGatewayResponse
    {
        public bool Success { get; set; }
        public string TransactionId { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
    }

}
