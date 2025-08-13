using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PetCare.Pets.Application.DTOs;
using PetCare.Pets.Application.Interfaces;
using PetCare.Shared.DTOs;

namespace PetCare.Pets.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // ✅ Puedes descomentar esto si ya tienes JWT funcionando
    public class PetsController : ControllerBase
    {
        private readonly IPetService _petService;

        public PetsController(IPetService petService)
        {
            _petService = petService;
        }

        /// <summary>
        /// Obtiene todas las mascotas registradas.
        /// </summary>
        /// <returns>Lista de mascotas.</returns>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<PetDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll()
        {
            var pets = await _petService.GetAllAsync();
            return Ok(pets);
        }

        /// <summary>
        /// Obtiene una mascota por su ID.
        /// </summary>
        /// <param name="id">ID de la mascota.</param>
        /// <returns>Datos de la mascota.</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(PetDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(Guid id)
        {
            var pet = await _petService.GetByIdAsync(id);
            return pet is null ? NotFound() : Ok(pet);
        }

        /// <summary>
        /// Crea una nueva mascota.
        /// </summary>
        /// <param name="dto">Datos de la mascota.</param>
        /// <returns>ID de la mascota creada.</returns>
        [HttpPost]
        [ProducesResponseType(typeof(object), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody] CreatePetDto dto)
        {
            if (!ModelState.IsValid)
                return ValidationProblem(ModelState);

            var petId = await _petService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = petId }, new { Id = petId });
        }

        /// <summary>
        /// Actualiza los datos de una mascota.
        /// </summary>
        /// <param name="id">ID de la mascota.</param>
        /// <param name="dto">Datos actualizados.</param>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdatePetDto dto)
        {
            if (!ModelState.IsValid)
                return ValidationProblem(ModelState);

            var updated = await _petService.UpdateAsync(id, dto);
            return updated ? NoContent() : NotFound();
        }

        /// <summary>
        /// Elimina una mascota por su ID.
        /// </summary>
        /// <param name="id">ID de la mascota.</param>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(Guid id)
        {
            var deleted = await _petService.DeleteAsync(id);
            return deleted ? NoContent() : NotFound();
        }
    }
}
