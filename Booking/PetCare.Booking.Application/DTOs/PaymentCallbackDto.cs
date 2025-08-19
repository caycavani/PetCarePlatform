
namespace PetCare.Booking.Application.DTOs
{
    public class PaymentCallbackDto
    {
        public Guid PaymentId { get; set; }
        public Guid BookingId { get; set; }
        public string Status { get; set; } // Completed, Failed, Refunded
        public string TransactionId { get; set; }
        public DateTime Timestamp { get; set; }
    }

}
