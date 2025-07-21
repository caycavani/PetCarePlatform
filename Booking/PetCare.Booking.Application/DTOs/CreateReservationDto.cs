namespace PetCare.Booking.Application.DTOs
{
    using System;

    public class CreateReservationDto
    {
        public Guid PetId { get; set; }
        public Guid CaregiverId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}
