using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PetCare.Booking.Application.DTOs;
using PetCare.Booking.Domain.Interfaces;

namespace PetCare.Booking.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PaymentCallbackController : ControllerBase
    {
        private readonly IReservationRepository _bookingRepository;
        private readonly ILogger<PaymentCallbackController> _logger;

        public PaymentCallbackController(IReservationRepository bookingRepository, ILogger<PaymentCallbackController> logger)
        {
            _bookingRepository = bookingRepository;
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> ReceivePaymentResult([FromBody] PaymentCallbackDto callback)
        {
            _logger.LogInformation("Recibido callback de pago: {@Callback}", callback);

            // Validar existencia de reserva
            var booking = await _bookingRepository.GetByIdAsync(callback.BookingId);
            if (booking == null)
            {
                _logger.LogWarning("Reserva no encontrada: {BookingId}", callback.BookingId);
                return NotFound($"Reserva {callback.BookingId} no existe.");
            }

            // Actualizar estado de reserva según resultado
            switch (callback.Status)
            {
                case "Completed":
                    booking.MarkAsPaid(callback.TransactionId, callback.Timestamp);
                    break;
                case "Failed":
                    booking.MarkAsPaymentFailed(callback.TransactionId, callback.Timestamp);
                    break;
                case "Refunded":
                    booking.MarkAsRefunded(callback.TransactionId, callback.Timestamp);
                    break;
                default:
                    _logger.LogWarning("Estado de pago desconocido: {Status}", callback.Status);
                    return BadRequest("Estado inválido.");
            }

            await _bookingRepository.UpdateAsync(booking);
            _logger.LogInformation("Reserva actualizada correctamente: {BookingId}", booking.Id);

            return Ok();
        }
    }

}
