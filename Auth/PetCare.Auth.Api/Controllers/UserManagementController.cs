using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System;

namespace PetCare.Auth.Api.Controllers
{
    [Authorize(Roles = "Admin")]
    [ApiController]
    [Route("api/admin/users")]
    public class UserManagementController : ControllerBase
    {
        [HttpGet("profile")]
        public IActionResult GetAdminProfile()
        {
            var email = User.FindFirst(ClaimTypes.Email)?.Value;
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var role = User.FindFirst(ClaimTypes.Role)?.Value;
            var jti = User.FindFirst(JwtRegisteredClaimNames.Jti)?.Value;

            return Ok(new
            {
                message = "Bienvenido, administrador ??",
                user = new
                {
                    Id = userId,
                    Email = email,
                    Role = role,
                    TokenId = jti
                }
            });
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteUser(Guid id)
        {
            // Aquí podrías llamar a un UserService o Repo para eliminar
            return Ok(new { message = $"Usuario con ID {id} fue eliminado (simulado)." });
        }
    }
}
