using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using PetCare.Auth.Application.DTOs.Auth;
using PetCare.Auth.Application.Interfaces;

namespace PetCare.Auth.Api.Controllers
{
    [ApiController]
    [Route("api/auth/me")]
    [Authorize]
    public class MeController : ControllerBase
    {
        private readonly IAuthService _authService;

        public MeController(IAuthService authService)
        {
            _authService = authService;
        }

        /// <summary>
        /// Obtiene el perfil del usuario autenticado usando los claims del JWT.
        /// </summary>
        [HttpGet]
        public IActionResult GetProfile()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var email = User.FindFirstValue(ClaimTypes.Email);
            var username = User.Identity?.Name;
            var role = User.FindFirstValue(ClaimTypes.Role);

            if (userId is null)
                return Unauthorized("Token inválido.");

            var profileDto = new
            {
                Id = userId,
                Email = email,
                Username = username,
                Role = role
            };

            return Ok(profileDto);
        }

        /// <summary>
        /// Actualiza el nombre completo y teléfono del usuario autenticado.
        /// </summary>
        [HttpPut]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileDto dto)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userIdClaim is null || !Guid.TryParse(userIdClaim, out var userId))
                return Unauthorized("Token inválido o corrupto.");

            await _authService.UpdateProfileAsync(userId, dto);
            return NoContent();
        }
    }
}
