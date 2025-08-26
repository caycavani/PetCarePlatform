namespace PetCare.Review.Infrastructure.Validators
{
    using System;
    using System.Threading.Tasks;
    using PetCare.Review.Domain.Entities;
    using PetCare.Review.Domain.Interfaces.kafka.Validation;

    public class ReviewValidator : IReviewValidator
    {
        public Task ValidateAsync(Review review)
        {
            if (review == null)
                throw new ArgumentNullException(nameof(review));

            if (review.Rating.Value < 1 || review.Rating.Value > 5)
                throw new ArgumentException("La calificación debe estar entre 1 y 5.");

            if (string.IsNullOrWhiteSpace(review.Comment.Value))
                throw new ArgumentException("El comentario no puede estar vacío.");

            if (review.ReservationId == Guid.Empty)
                throw new ArgumentException("ReservationId no puede estar vacío.");

            return Task.CompletedTask;
        }
    }
}
