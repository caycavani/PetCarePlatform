using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PetCare.Booking.Domain.DTOs;
using PetCare.Booking.Domain.Entities;
using PetCare.Booking.Domain.Interfaces;

namespace PetCare.Booking.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ServiceController : ControllerBase
    {
        private readonly IServiceRepository _repository;

        public ServiceController(IServiceRepository repository)
        {
            _repository = repository;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateServiceRequest request)
        {
            var service = new Service
            {
                Id = Guid.NewGuid(),
                Name = request.Name,
                Description = request.Description,
                CaregiverId = request.CaregiverId
            };

            var created = await _repository.CreateAsync(service);
            if (!created)
                return BadRequest("No se pudo crear el servicio.");

            return CreatedAtAction(nameof(GetById), new { id = service.Id }, service);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var service = await _repository.GetByIdAsync(id);
            if (service == null)
                return NotFound();

            return Ok(service);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var services = await _repository.GetAllAsync();
            return Ok(services);
        }
    }
}
