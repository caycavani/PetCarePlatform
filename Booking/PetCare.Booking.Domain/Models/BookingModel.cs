namespace PetCare.Booking.Domain.Models
{
    using System;
    
    /// <summary>
    /// Represents a data transfer model for a booking.
    /// </summary>
    public class BookingModel
    {
        /// <summary>
        /// Gets or sets the unique identifier of the booking.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the identifier of the pet being booked.
        /// </summary>
        public Guid PetId { get; set; }

        /// <summary>
        /// Gets or sets the identifier of the assigned caregiver.
        /// </summary>
        public Guid CaregiverId { get; set; }

        /// <summary>
        /// Gets or sets the start date of the booking.
        /// </summary>
        public DateTime StartDate { get; set; }

        /// <summary>
        /// Gets or sets the end date of the booking.
        /// </summary>
        public DateTime EndDate { get; set; }

        /// <summary>
        /// Gets or sets the current status of the booking.
        /// </summary>
        public string Status { get; set; } = string.Empty;
    }
}
