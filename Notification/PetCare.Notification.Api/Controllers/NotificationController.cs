using Microsoft.AspNetCore.Mvc;
using PetCare.Notification.Api.Models;
using PetCare.Notification.Domain.Interfaces;
using System.ComponentModel.DataAnnotations;

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
}
