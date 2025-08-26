namespace PetCare.Review.Application.DTOs
{
    using System.ComponentModel.DataAnnotations;

    public class CreateReviewDto
    {
        [Required]
        public Guid ReservationId { get; set; }

        [Required]
        public Guid AuthorId { get; set; }

        [Range(1, 5)]
        public int Rating { get; set; }

        [Required]
        [MaxLength(500)]
        public string Comment { get; set; }
    }
}
