namespace PetCare.Review.Infrastructure.Publishers
{
    using System.Threading.Tasks;
    using Microsoft.Extensions.Logging;
    using PetCare.Review.Domain.Entities;
    using PetCare.Review.Domain.Interfaces.kafka.Eventos;

    public class ReviewPublisher : IReviewPublisher
    {
        private readonly ILogger<ReviewPublisher> _logger;

        public ReviewPublisher(ILogger<ReviewPublisher> logger)
        {
            _logger = logger;
        }

        public Task PublishCreatedAsync(Review review)
        {
            // Aquí iría la lógica real de publicación a Kafka o cualquier bus de eventos
            _logger.LogInformation("Simulando publicación de evento review.created para ID {ReviewId}", review.Id);
            return Task.CompletedTask;
        }
    }
}
