namespace PetCare.Review.Application.Services
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Logging;
    using PetCare.Review.Application.Interfaces;
    using PetCare.Review.Domain.Entities;
    using PetCare.Review.Domain.Interfaces;
    using PetCare.Review.Domain.Interfaces.kafka.Eventos;
    using PetCare.Review.Domain.Interfaces.kafka.Validation;

    public class ReviewService : IReviewService
    {
        private readonly IReviewRepository _repository;
        private readonly IReviewValidator _validator;
        private readonly IReviewPublisher _publisher;
        private readonly ILogger<ReviewService> _logger;

        public ReviewService(
            IReviewRepository repository,
            IReviewValidator validator,
            IReviewPublisher publisher,
            ILogger<ReviewService> logger)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _validator = validator ?? throw new ArgumentNullException(nameof(validator));
            _publisher = publisher ?? throw new ArgumentNullException(nameof(publisher));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<Review> CreateAsync(Review review)
        {
            _logger.LogInformation("Validando reseña para reserva {ReservationId}", review.ReservationId);
            await _validator.ValidateAsync(review);

            var exists = await _repository.ExistsByReservationAsync(review.ReservationId);
            if (exists)
            {
                _logger.LogWarning("Ya existe una reseña para la reserva {ReservationId}", review.ReservationId);
                throw new InvalidOperationException("Ya existe una reseña para esta reserva.");
            }

            await _repository.AddAsync(review);
            _logger.LogInformation("Reseña creada con ID {ReviewId}", review.Id);

            await _publisher.PublishCreatedAsync(review);
            _logger.LogInformation("Evento review.created publicado para ID {ReviewId}", review.Id);

            return review;
        }

        public async Task<Review?> GetByIdAsync(Guid reviewId)
        {
            _logger.LogDebug("Consultando reseña por ID {ReviewId}", reviewId);
            return await _repository.GetByIdAsync(reviewId);
        }

        public async Task<IEnumerable<Review>> GetByReservationAsync(Guid reservationId)
        {
            _logger.LogDebug("Consultando reseñas por reserva {ReservationId}", reservationId);
            return await _repository.GetByReservationAsync(reservationId);
        }

        public async Task<IEnumerable<Review>> GetAllAsync()
        {
            _logger.LogDebug("Consultando todas las reseñas");
            return await _repository.GetAllAsync();
        }

        public async Task DeleteAsync(Guid reviewId)
        {
            _logger.LogInformation("Eliminando reseña con ID {ReviewId}", reviewId);
            await _repository.DeleteAsync(reviewId);
        }
    }
}
