using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using PetCare.Pets.Application.DTOs;
using PetCare.Pets.Application.Interfaces;
using PetCare.Shared.DTOs;
using PetCare.Shared.DTOs.Utils;
using System.Security.Claims;

namespace PetCare.Pets.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(AuthenticationSchemes = "Bearer")]
    public class PetsController : ControllerBase
    {
        private readonly IPetService _petService;

        public PetsController(IPetService petService)
        {
            _petService = petService;
        }

        /// <summary>
        /// Obtiene todas las mascotas registradas del usuario autenticado.
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<PetDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(userId, out var ownerId))
                return Unauthorized("No se pudo identificar al usuario.");

            var pets = await _petService.GetAllByOwnerAsync(ownerId);
            return Ok(pets);
        }

        /// <summary>
        /// Obtiene una mascota por su ID.
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(PetDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(Guid id)
        {
            var pet = await _petService.GetByIdAsync(id);
            return pet is null ? NotFound() : Ok(pet);
        }

        /// <summary>
        /// Crea una nueva mascota asociada al usuario autenticado.
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(object), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody] CreatePetDto dto)
        {
            if (!ModelState.IsValid)
                return ValidationProblem(ModelState);

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(userId, out var ownerId))
                return Unauthorized("No se pudo identificar al usuario.");

            dto.OwnerId = ownerId;

            var petId = await _petService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = petId }, new { Id = petId });
        }

        /// <summary>
        /// Actualiza los datos de una mascota.
        /// </summary>
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
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(Guid id)
        {
            var deleted = await _petService.DeleteAsync(id);
            return deleted ? NoContent() : NotFound();
        }

        [HttpGet("api/debug/jwt-options")]
        [AllowAnonymous]
        public IActionResult GetJwtOptions([FromServices] IOptions<JwtBearerOptions> jwtOptions)
        {
            var key = jwtOptions.Value.TokenValidationParameters.IssuerSigningKey;
            var issuer = jwtOptions.Value.TokenValidationParameters.ValidIssuer;
            var audience = jwtOptions.Value.TokenValidationParameters.ValidAudience;

            return Ok(new
            {
                KeyType = key?.GetType().Name,
                Issuer = issuer,
                Audience = audience
            });
        }




    }
}
