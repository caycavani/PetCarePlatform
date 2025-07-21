using Microsoft.AspNetCore.Mvc;
using PetCare.Notification.Application.DTOs;
using PetCare.Notification.Application.UseCases;

namespace PetCare.Notification.Api.Controllers
{
    /// <summary>
    /// Controlador para gestionar el env�o de notificaciones.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class NotificationController : ControllerBase
    {
        private readonly SendNotificationUseCase _useCase;

        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="NotificationController"/>.
        /// </summary>
        /// <param name="useCase">Caso de uso para enviar notificaciones.</param>
        public NotificationController(SendNotificationUseCase useCase)
        {
            _useCase = useCase;
        }

        /// <summary>
        /// Env�a una nueva notificaci�n al destinatario especificado.
        /// </summary>
        /// <param name="dto">Datos de la notificaci�n.</param>
        /// <returns>El identificador de la notificaci�n creada.</returns>
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] CreateNotificationDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var id = await _useCase.ExecuteAsync(dto);
            return Ok(new { Id = id });
        }
    }
}
