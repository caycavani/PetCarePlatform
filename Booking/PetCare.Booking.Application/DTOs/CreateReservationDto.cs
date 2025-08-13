namespace PetCare.Booking.Application.DTOs
{
    public class CreateReservationDto
    {
        public Guid ClientId { get; set; }
        public Guid CaregiverId { get; set; } // Este lo asigna el servicio, puede omitirse en el request
        public Guid PetId { get; set; }
        public Guid ServiceId { get; set; } // ✅ Este campo es obligatorio
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string? Note { get; set; }
    }
}
