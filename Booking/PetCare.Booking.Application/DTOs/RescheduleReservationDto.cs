namespace PetCare.Booking.Application.DTOs
{
    using System.ComponentModel.DataAnnotations;

    public class RescheduleReservationDto
    {
        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }
    }

}
