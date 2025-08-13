namespace PetCare.Booking.Application.DTOs
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class ReservationDetailsDto
    {
        public Guid Id { get; set; }

        public Guid PetId { get; set; }

        public Guid CaregiverId { get; set; }

        public Guid ClientId { get; set; }

        public string StatusName { get; set; } = default!;

        public string DisplayColor { get; set; } = "#000000";

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public string? Notes { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }
    }

}
