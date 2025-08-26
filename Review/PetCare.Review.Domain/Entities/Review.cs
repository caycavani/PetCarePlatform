namespace PetCare.Review.Domain.Entities
{
    using PetCare.Review.Domain.ValueObjects;
    using System;

    public class Review
    {
        public Guid Id { get; private set; }
        public Guid ReservationId { get; private set; }
        public Guid AuthorId { get; private set; }
        public Rating Rating { get; private set; }
        public Comment Comment { get; private set; }
        public DateTime CreatedAt { get; private set; }

        // Constructor protegido para EF Core
        protected Review() { }

        // Constructor público para creación desde Application/API
        public Review(Guid reservationId, Guid authorId, Rating rating, Comment comment)
        {
            if (reservationId == Guid.Empty)
                throw new ArgumentException("ReservationId cannot be empty.", nameof(reservationId));

            if (authorId == Guid.Empty)
                throw new ArgumentException("AuthorId cannot be empty.", nameof(authorId));

            Id = Guid.NewGuid();
            ReservationId = reservationId;
            AuthorId = authorId;
            Rating = rating ?? throw new ArgumentNullException(nameof(rating));
            Comment = comment ?? throw new ArgumentNullException(nameof(comment));
            CreatedAt = DateTime.UtcNow;
        }
    }
}
