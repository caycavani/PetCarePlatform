using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace PetCare.Auth.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/debug/token")]
    public class TokenDebugController : ControllerBase
    {
        [HttpGet]
        public IActionResult GetTokenClaims()
        {
            var claims = User.Claims
                .Select(c => new { Type = c.Type, Value = c.Value })
                .ToList();

            return Ok(new
            {
                message = "Claims extraídos del token actual 🎯",
                claims = claims
            });
        }
    }
}
