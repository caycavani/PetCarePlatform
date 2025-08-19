namespace PetCares.Shared.DTOs.DTOs.Payment.Responses
{
    public class RefundResult
    {
        public string TransactionId { get; set; } = string.Empty;
        public bool Refunded { get; set; }
        public string Message { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; }
    }


}
