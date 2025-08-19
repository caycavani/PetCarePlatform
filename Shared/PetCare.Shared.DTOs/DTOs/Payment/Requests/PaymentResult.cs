namespace PetCare.Shared.DTOs.Payment.requests
{
    public class PaymentResult
    {
        public bool Success { get; set; }
        public string TransactionId { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public DateTime ProcessedAt { get; set; }
        public Dictionary<string, string>? Metadata { get; set; }
    }
}
