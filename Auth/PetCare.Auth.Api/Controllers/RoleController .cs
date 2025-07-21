using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PetCare.Auth.Api.Models;
using PetCare.Auth.Application.Interfaces;
using System;
using System.Threading.Tasks;

namespace PetCare.Auth.Api.Controllers
{
    [ApiController]
    [Route("api/roles")]
    public class RoleController : ControllerBase
    {
        private readonly IRoleService _roleService;

        public RoleController(IRoleService roleService)
        {
            _roleService = roleService;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll()
        {
            var roles = await _roleService.GetAllAsync();
            return Ok(roles);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(Guid id)
        {
            var role = await _roleService.GetByIdAsync(id);
            return Ok(role);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody] CreateRoleRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            await _roleService.CreateAsync(request.Name, request.Description);
            return Created("", null);
        }

        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Update(Guid id, [FromBody] CreateRoleRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            await _roleService.UpdateAsync(id, request.Name, request.Description);
            return NoContent();
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _roleService.DeleteAsync(id);
            return NoContent();
        }
    }
}
