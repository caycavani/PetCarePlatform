using Microsoft.AspNetCore.Mvc;
using PetCare.Pets.Application.DTOs;
using PetCare.Pets.Application.UseCases;

namespace PetCare.Pets.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PetsController : ControllerBase
    {
        private readonly RegisterPetUseCase _registerPetUseCase;

        public PetsController(RegisterPetUseCase registerPetUseCase)
        {
            _registerPetUseCase = registerPetUseCase;
        }

        /// <summary>
        /// Registra una nueva mascota asociada a un dueño.
        /// </summary>
        /// <param name="dto">Datos de la mascota.</param>
        /// <returns>El ID de la mascota creada.</returns>
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] CreatePetDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var petId = await _registerPetUseCase.ExecuteAsync(dto);
            return Ok(new { Id = petId });
        }
    }
}
