namespace PetCare.Review.Application.DTOs
{
    public class ReviewResponseDto
    {
        public Guid Id { get; set; }
        public Guid ReservationId { get; set; }
        public Guid AuthorId { get; set; }
        public int Rating { get; set; }
        public string Comment { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
