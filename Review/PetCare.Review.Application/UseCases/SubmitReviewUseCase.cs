namespace PetCare.Review.Application.UseCases
{
    using PetCare.Review.Application.DTOs;
    using PetCare.Review.Domain.Entities;
    using PetCare.Review.Domain.Interfaces;

    public class SubmitReviewUseCase
    {
        private readonly IReviewRepository _repository;

        public SubmitReviewUseCase(IReviewRepository repository)
        {
            _repository = repository;
        }

        public async Task<Guid> ExecuteAsync(CreateReviewDto dto)
        {
            var review = new Rview(
                reservationId: dto.ReservationId,
                authorId: dto.AuthorId,
                rating: dto.Rating,
                comment: dto.Comment
            );

            await _repository.AddAsync(review);

            return review.Id;
        }
    }
}
