using Microsoft.AspNetCore.Mvc;
using PetCare.Pets.Application.DTOs;
using PetCare.Pets.Application.UseCases;

namespace PetCare.Pets.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PetController : ControllerBase
    {
        private readonly RegisterPetUseCase _useCase;

        public PetController(RegisterPetUseCase useCase)
        {
            _useCase = useCase;
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] CreatePetDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var id = await _useCase.ExecuteAsync(dto);
            return Ok(new { Id = id });
        }
    }
}
