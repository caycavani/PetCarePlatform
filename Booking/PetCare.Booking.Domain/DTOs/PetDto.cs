namespace PetCare.Booking.Domain.DTOs
{
    public class PetDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public DateTime BirthDate { get; set; }
        public string Breed { get; set; } = string.Empty;
        public int Age { get; set; }
        public Guid OwnerId { get; set; } // ✅ Este campo es clave para Booking
    }

}
