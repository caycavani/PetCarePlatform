namespace PetCare.Review.Api.Controllers
{
    using Microsoft.AspNetCore.Authentication.JwtBearer;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Options;
    using PetCare.Review.Application.DTOs;
    using PetCare.Review.Application.Interfaces;
    using PetCare.Review.Domain.Entities;
    using PetCare.Review.Domain.ValueObjects;
    using System.ComponentModel.DataAnnotations;
    using System.Security.Claims;

    [ApiController]
    [Route("api/v1/reviews")]
    [Produces("application/json")]
    public class ReviewController : ControllerBase
    {
        private readonly IReviewService _service;
        private readonly JwtBearerOptions _jwtOptions;

        public ReviewController(
            IReviewService service,
            IOptionsMonitor<JwtBearerOptions> jwtOptionsMonitor)
        {
            _service = service;
            _jwtOptions = jwtOptionsMonitor.Get(JwtBearerDefaults.AuthenticationScheme);
        }

        /// <summary>
        /// Crea una nueva reseña.
        /// </summary>
        [Authorize]
        [HttpPost]
        [Consumes("application/json")]
        [ProducesResponseType(typeof(ReviewResponseDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreateAsync([FromBody] CreateReviewDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var authorIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!Guid.TryParse(authorIdClaim, out var authenticatedAuthorId))
                return Unauthorized("No se pudo identificar al usuario autenticado.");

            if (authenticatedAuthorId != dto.AuthorId)
                return BadRequest("El AuthorId no coincide con el usuario autenticado.");

            try
            {
                var rating = new Rating(dto.Rating);
                var comment = new Comment(dto.Comment);
                var review = new Review(dto.ReservationId, authenticatedAuthorId, rating, comment);

                var created = await _service.CreateAsync(review);

                var result = new ReviewResponseDto
                {
                    Id = created.Id,
                    ReservationId = created.ReservationId,
                    AuthorId = created.AuthorId,
                    Rating = created.Rating.Value,
                    Comment = created.Comment.Value,
                    CreatedAt = created.CreatedAt
                };

                // ✅ Usamos CreatedAtRoute con nombre explícito
                return CreatedAtRoute("GetReviewById", new { reviewId = result.Id }, result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno: {ex.Message}");
            }
        }

        /// <summary>
        /// Obtiene una reseña por ID.
        /// </summary>
        [Authorize]
        [HttpGet("{reviewId:guid}", Name = "GetReviewById")]
        [ProducesResponseType(typeof(ReviewResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetByIdAsync(Guid reviewId)
        {
            var review = await _service.GetByIdAsync(reviewId);
            if (review is null) return NotFound();

            var result = new ReviewResponseDto
            {
                Id = review.Id,
                ReservationId = review.ReservationId,
                AuthorId = review.AuthorId,
                Rating = review.Rating.Value,
                Comment = review.Comment.Value,
                CreatedAt = review.CreatedAt
            };

            return Ok(result);
        }

        /// <summary>
        /// Lista todas las reseñas.
        /// </summary>
        [HttpGet("all")]
        [ProducesResponseType(typeof(IEnumerable<ReviewResponseDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllAsync()
        {
            var reviews = await _service.GetAllAsync();

            var result = reviews.Select(r => new ReviewResponseDto
            {
                Id = r.Id,
                ReservationId = r.ReservationId,
                AuthorId = r.AuthorId,
                Rating = r.Rating.Value,
                Comment = r.Comment.Value,
                CreatedAt = r.CreatedAt
            });

            return Ok(result);
        }

        /// <summary>
        /// Elimina una reseña por ID.
        /// </summary>
        [Authorize]
        [HttpDelete("{reviewId:guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteAsync(Guid reviewId)
        {
            var review = await _service.GetByIdAsync(reviewId);
            if (review is null) return NotFound();

            await _service.DeleteAsync(reviewId);
            return NoContent();
        }

        /// <summary>
        /// Devuelve los parámetros de validación JWT configurados.
        /// </summary>
        [HttpGet("jwt-info")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult GetJwtValidationParameters()
        {
            var parameters = _jwtOptions.TokenValidationParameters;

            return Ok(new
            {
                parameters.ValidIssuer,
                parameters.ValidAudience,
                parameters.ValidateIssuer,
                parameters.ValidateAudience,
                parameters.ValidateLifetime,
                parameters.ValidateIssuerSigningKey
            });
        }

        /// <summary>
        /// Devuelve los claims del usuario autenticado.
        /// </summary>
        [HttpGet("jwt-user")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult GetJwtUser()
        {
            var claims = new
            {
                NameIdentifier = User.FindFirstValue(ClaimTypes.NameIdentifier),
                Name = User.FindFirstValue(ClaimTypes.Name),
                Email = User.FindFirstValue(ClaimTypes.Email),
                Role = User.FindFirstValue(ClaimTypes.Role),
                Issuer = User.FindFirstValue("iss"),
                Audience = User.FindFirstValue("aud")
            };

            return Ok(claims);
        }
    }
}
