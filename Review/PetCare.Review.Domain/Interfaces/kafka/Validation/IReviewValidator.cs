namespace PetCare.Review.Domain.Interfaces.kafka.Validation
{
    using System.Threading.Tasks;
    using PetCare.Review.Domain.Entities;

    public interface IReviewValidator
    {
        Task ValidateAsync(Review review);
    }
}
