using Microsoft.AspNetCore.Mvc;
using PetCare.Auth.Application.DTOs.Auth;
using PetCare.Auth.Application.DTOs.User;
using PetCare.Auth.Application.Interfaces;

namespace PetCare.Auth.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        /// <summary>
        /// Registra un nuevo usuario con el rol especificado.
        /// </summary>
        [HttpPost("register")]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Register([FromBody] CreateUserDto dto, [FromQuery] string role = "user")
        {
            if (!ModelState.IsValid) return ValidationProblem(ModelState);

            await _authService.RegisterAsync(dto, role);
            return Ok(new { Message = "Usuario registrado exitosamente." });
        }

        /// <summary>
        /// Autentica a un usuario y devuelve los tokens.
        /// </summary>
        [HttpPost("login")]
        [ProducesResponseType(typeof(LoginResultDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Login([FromBody] LoginUserDto dto)
        {
            if (!ModelState.IsValid) return ValidationProblem(ModelState);

            try
            {
                var result = await _authService.AuthenticateAsync(dto);
                return Ok(result);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
        }

        /// <summary>
        /// Solicita nuevos tokens usando el refresh token.
        /// </summary>
        [HttpPost("refresh")]
        [ProducesResponseType(typeof(TokenResultDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Refresh([FromQuery] Guid userId, [FromQuery] string refreshToken)
        {
            if (string.IsNullOrWhiteSpace(refreshToken))
                return BadRequest("El refresh token es obligatorio.");

            try
            {
                var result = await _authService.RefreshTokenAsync(userId, refreshToken);
                return Ok(result);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
        }


    }
}
