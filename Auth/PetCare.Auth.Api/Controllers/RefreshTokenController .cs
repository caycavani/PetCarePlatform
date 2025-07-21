using Microsoft.AspNetCore.Mvc;
using PetCare.Auth.Application.DTOs.Jwt;
using PetCare.Auth.Application.Interfaces;
using System.Threading.Tasks;
using System;
using System.Threading.Tasks;

namespace PetCare.Auth.Api.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class RefreshTokenController : ControllerBase
    {
        private readonly IRefreshTokenService _refreshTokenService;

        public RefreshTokenController(IRefreshTokenService refreshTokenService)
        {
            _refreshTokenService = refreshTokenService;
        }

        /// <summary>
        /// Renueva el token de acceso usando un refresh token válido y no revocado.
        /// </summary>
        /// <param name="refreshToken">Refresh token vigente del usuario</param>
        /// <returns>Par de tokens: AccessToken y nuevo RefreshToken</returns>
        [HttpPost("refresh-token")]
        [ProducesResponseType(typeof(JwtDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> RenewToken([FromBody] string refreshToken)
        {
            if (string.IsNullOrWhiteSpace(refreshToken))
                return BadRequest(new { error = "El refresh token no puede estar vacío." });

            try
            {
                var tokenPair = await _refreshTokenService.RenewAccessTokenAsync(refreshToken);
                return Ok(tokenPair);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = $"Error interno: {ex.Message}" });
            }
        }
    }
}
