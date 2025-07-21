using Microsoft.AspNetCore.Mvc;
using PetCare.Review.Application.DTOs;
using PetCare.Review.Application.UseCases;

namespace PetCare.Review.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReviewsController : ControllerBase
    {
        private readonly SubmitReviewUseCase _submitReviewUseCase;

        public ReviewsController(SubmitReviewUseCase submitReviewUseCase)
        {
            _submitReviewUseCase = submitReviewUseCase;
        }

        /// <summary>
        /// Registra una rese�a asociada a una reserva.
        /// </summary>
        /// <param name="dto">Datos de la rese�a.</param>
        /// <returns>ID de la rese�a creada.</returns>
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] CreateReviewDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var reviewId = await _submitReviewUseCase.ExecuteAsync(dto);
            return Ok(new { Id = reviewId });
        }
    }
}
