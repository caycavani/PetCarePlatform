using PetCare.Auth.Application.DTOs.Jwt;
using System.Threading.Tasks;
using System.Threading.Tasks;

namespace PetCare.Auth.Application.Interfaces
{
    public interface IRefreshTokenService
    {
        /// <summary>
        /// Renueva el token de acceso y retorna también un nuevo refresh token.
        /// </summary>
        Task<JwtDto> RenewAccessTokenAsync(string refreshToken);
    }
}
