namespace PetCare.Review.Domain.Entities
{
    using System;

    public class Rview
    {
        public Guid Id { get; private set; }
        public Guid ReservationId { get; private set; }
        public Guid AuthorId { get; private set; }
        public int Rating { get; private set; }
        public string Comment { get; private set; }
        public DateTime CreatedAt { get; private set; }

        protected Rview() { }

        public Rview(Guid reservationId, Guid authorId, int rating, string comment)
        {
            Id = Guid.NewGuid();
            ReservationId = reservationId;
            AuthorId = authorId;
            Rating = rating;
            Comment = comment;
            CreatedAt = DateTime.UtcNow;
        }
    }
}
