using Microsoft.AspNetCore.Mvc;
using PetCare.Shared.DTOs;
using System.Net.Http;
using System.Net.Http.Json;

namespace PetCare.Payment.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PaymentController : ControllerBase
{
    private readonly HttpClient _httpClient;

    public PaymentController(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    [HttpPost("register-pet")]
    public async Task<IActionResult> RegisterPetAsync([FromBody] CreatePetRequestDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            // 🚀 Enviar el DTO al microservicio de Pets vía HTTP POST
            var response = await _httpClient.PostAsJsonAsync("/api/pet", dto);

            if (!response.IsSuccessStatusCode)
                return StatusCode((int)response.StatusCode, "Error al registrar la mascota en Pets.Api");

            // 🧾 Leer el contenido retornado (por ejemplo, el Id generado)
            var content = await response.Content.ReadFromJsonAsync<object>();
            return Ok(content);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error al consumir Pets.Api: {ex.Message}");
        }
    }
}
