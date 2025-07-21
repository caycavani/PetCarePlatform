using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PetCare.Auth.Application.DTOs.User;
using PetCare.Auth.Application.Interfaces;
using System.Security.Claims;

namespace PetCare.Auth.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // Protege todos los endpoints por defecto
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        /// <summary>
        /// Obtiene el perfil del usuario autenticado.
        /// </summary>
        [HttpGet("me")]
        [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetMyProfile()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(userId) || !Guid.TryParse(userId, out var guid))
                return Unauthorized("Token inválido o sin identificador.");

            var user = await _userService.GetByIdAsync(guid);
            return Ok(user);
        }

        /// <summary>
        /// Actualiza el perfil del usuario autenticado.
        /// </summary>
        [HttpPut("me")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> UpdateMyProfile([FromBody] UpdateUserDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(userId) || !Guid.TryParse(userId, out var guid))
                return Unauthorized("Token inválido o sin identificador.");

            dto.Id = guid;
            await _userService.UpdateAsync(dto);
            return NoContent();
        }

        /// <summary>
        /// (Solo admins) Lista todos los usuarios.
        /// </summary>
        [HttpGet]
        [Authorize(Roles = "admin")] // Solo usuarios con rol 'admin'
        [ProducesResponseType(typeof(List<UserDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllUsers()
        {
            // Si agregas un método como GetAllAsync en IUserService, puedes usarlo aquí
            throw new NotImplementedException("Método GetAllUsers aún no implementado.");
        }
    }
}
