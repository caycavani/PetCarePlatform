using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PetCare.Booking.Application.Extensions;
using PetCare.Booking.Application.Interfaces;
using PetCare.Booking.Domain.DTOs;
using PetCare.Booking.Domain.Entities;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace PetCare.Booking.Controllers
{
    [ApiController]
    [Route("api/reservations")]
    [Authorize]
    public class ReservationController : ControllerBase
    {
        private readonly IReservationService _service;
        private readonly ILogger<ReservationController> _logger;

        public ReservationController(IReservationService service, ILogger<ReservationController> logger)
        {
            _service = service;
            _logger = logger;
        }

        [HttpPost]
        [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateReservation([FromBody][Required] CreateReservationRequest request)
        {
            if (request == null || request.ServiceId == Guid.Empty || request.PetId == Guid.Empty)
            {
                _logger.LogWarning("Solicitud inválida: ServiceId o PetId faltantes.");
                return BadRequest("Solicitud inválida. Debes incluir ServiceId y PetId.");
            }

            var clientId = User.GetUserId();
            _logger.LogInformation("Creando reserva para cliente {ClientId} con mascota {PetId}", clientId, request.PetId);

            var reservationDto = new CreateReservationDto
            {
                PetId = request.PetId,
                ServiceId = request.ServiceId,
                StartDate = request.StartDate,
                EndDate = request.EndDate,
                Note = request.Note,
                ClientId = clientId
            };

            var reservationId = await _service.CreateAsync(reservationDto);

            _logger.LogInformation("Reserva creada con ID {ReservationId}", reservationId);
            return CreatedAtAction(nameof(GetReservationById), new { id = reservationId }, new { id = reservationId });
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ReservationResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetReservationById(Guid id)
        {
            var userId = User.GetUserId();
            _logger.LogInformation("Consultando reserva {ReservationId} para usuario {UserId}", id, userId);

            var reservation = await _service.GetByIdAsync(id);

            if (reservation is null)
            {
                _logger.LogWarning("Reserva {ReservationId} no encontrada", id);
                return NotFound();
            }

            if (reservation.ClientId != userId)
            {
                _logger.LogWarning("Acceso denegado a reserva {ReservationId} para usuario {UserId}", id, userId);
                return Forbid();
            }

            return Ok(new ReservationResponse
            {
                Id = reservation.Id,
                Note = reservation.Note,
                StartDate = reservation.StartDate,
                EndDate = reservation.EndDate,
                ServiceId = reservation.ServiceId,
                PetId = reservation.PetId
            });
        }

        [HttpPut("{id}/note")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateNote(Guid id, [FromBody][Required] UpdateNoteRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Note))
            {
                _logger.LogWarning("Nota vacía para reserva {ReservationId}", id);
                return BadRequest("La nota no puede estar vacía.");
            }

            var userId = User.GetUserId();
            var reservation = await _service.GetByIdAsync(id);

            if (reservation is null)
            {
                _logger.LogWarning("Reserva {ReservationId} no encontrada para actualización de nota", id);
                return NotFound();
            }

            if (reservation.ClientId != userId)
            {
                _logger.LogWarning("Acceso denegado a actualización de nota en reserva {ReservationId} para usuario {UserId}", id, userId);
                return Forbid();
            }

            if (reservation.ReservationStatusId == ReservationStatus.Cancelled.Id)
            {
                _logger.LogWarning("Intento de actualizar nota en reserva cancelada {ReservationId}", id);
                return BadRequest("No se puede actualizar una reserva cancelada.");
            }

            var result = await _service.UpdateNoteAsync(id, request.Note);

            if (result)
            {
                _logger.LogInformation("Nota actualizada en reserva {ReservationId}", id);
                return NoContent();
            }

            _logger.LogWarning("Falló la actualización de nota en reserva {ReservationId}", id);
            return NotFound();
        }
    }
}
