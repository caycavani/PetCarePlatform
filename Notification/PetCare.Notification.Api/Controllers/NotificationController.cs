using Microsoft.AspNetCore.Mvc;
using PetCare.Notification.Api.Models;
using PetCare.Notification.Domain.Interfaces;
using PetCare.Shared.DTOs.Utils;
using System.Text.Json;

namespace PetCare.Notification.Api.Controllers;

/// <summary>
/// Controlador principal para gestionar notificaciones.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class NotificationController : ControllerBase
{
    private readonly INotificationProducer _producer;

    public NotificationController(INotificationProducer producer)
    {
        _producer = producer;
    }

    /// <summary>
    /// Publica una nueva notificación en Kafka.
    /// </summary>
    /// <param name="request">Datos de la notificación.</param>
    /// <returns>Resultado de la operación.</returns>
    [HttpPost]
    public async Task<IActionResult> PublishNotification([FromBody] NotificationRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var message = $"Notificación para {request.Recipient} | Canal: {request.Channel} | Mensaje: {request.Message}";
            await _producer.PublishAsync(message);

            return Ok(new
            {
                Status = "Publicado",
                Topic = "notification-events",
                Recipient = request.Recipient,
                Timestamp = DateTime.UtcNow
            });
        }
        catch (Exception ex)
        {
            var errorDto = ExceptionMapper.Map(ex);
            var jsonError = JsonSerializer.Serialize(errorDto);

            // Puedes loguear aquí o enviar a Kafka si lo deseas
            Console.Error.WriteLine(jsonError);

            return StatusCode(500, new
            {
                Error = "Error interno al publicar la notificación",
                Details = errorDto
            });
        }
    }
}
