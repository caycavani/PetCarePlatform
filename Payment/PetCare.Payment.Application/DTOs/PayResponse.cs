namespace PetCare.Payment.Application.DTOs
{
    public class PayResponse
    {
        public Guid Id { get; set; }
        public Guid ReservationId { get; set; }
        public decimal Amount { get; set; }

        public string Method { get; set; }
        public string Status { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime? CompletedAt { get; set; }
    }
}
