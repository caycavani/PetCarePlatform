using Microsoft.AspNetCore.Mvc;
using PetCare.Booking.Application.DTOs;
using PetCare.Booking.Domain.Entities;

namespace PetCare.Booking.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReservationController : ControllerBase
    {
        private readonly IReservationRepository _repository;

        public ReservationController(IReservationRepository repository)
        {
            _repository = repository;
        }

        [HttpPost]
        public async Task<IActionResult> CreateReservation([FromBody] CreateReservationDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var reservation = new Reservation(
                petId: dto.PetId,
                caregiverId: dto.CaregiverId,
                startDate: dto.StartDate,
                endDate: dto.EndDate
            );

            await _repository.AddAsync(reservation);

            return CreatedAtAction(nameof(GetReservationById), new { id = reservation.Id }, reservation);
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetReservationById(Guid id)
        {
            var reservation = await _repository.GetByIdAsync(id);
            if (reservation is null)
                return NotFound();

            return Ok(reservation);
        }
    }
}
