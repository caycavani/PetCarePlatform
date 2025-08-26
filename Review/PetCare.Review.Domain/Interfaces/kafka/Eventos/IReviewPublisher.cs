namespace PetCare.Review.Domain.Interfaces.kafka.Eventos
{
    using System.Threading.Tasks;
    using PetCare.Review.Domain.Entities;

    public interface IReviewPublisher
    {
        Task PublishCreatedAsync(Review review);
    }
}
