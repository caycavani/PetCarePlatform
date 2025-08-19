namespace PetCare.Payment.Application.DTOs
{
    public class TransactionDetails
    {
        public string TransactionId { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string Currency { get; set; } = "COP";
        public string Status { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime? CompletedAt { get; set; }
        public string GatewayName { get; set; } = string.Empty;
    }


}
