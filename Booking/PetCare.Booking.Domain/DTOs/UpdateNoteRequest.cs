namespace PetCare.Booking.Domain.DTOs
{
    public class UpdateNoteRequest
    {
        public Guid UserId { get; set; }
        public string Note { get; set; } = string.Empty;
    }
}
