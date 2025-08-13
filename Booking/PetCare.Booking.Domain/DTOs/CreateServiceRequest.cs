namespace PetCare.Booking.Domain.DTOs
{
    public class CreateServiceRequest
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public Guid CaregiverId { get; set; }
    }
}
