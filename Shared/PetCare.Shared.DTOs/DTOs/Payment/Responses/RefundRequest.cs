

namespace PetCare.Shared.DTOs.DTOs.Payment.Responses
{
    public class RefundRequest
    {
        public string TransactionId { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string Reason { get; set; } = string.Empty;
        public Guid RequestedBy { get; set; } // Puede ser el ID del usuario o del sistema
    }
}
