using PetCare.Auth.Application.DTOs.Auth;
using PetCare.Auth.Application.DTOs.User;
using System;
using System.Threading.Tasks;

namespace PetCare.Auth.Application.Interfaces
{
    public interface IAuthService
    {
        /// <summary>
        /// Autentica credenciales y genera tokens.
        /// </summary>
        Task<LoginResultDto> AuthenticateAsync(LoginUserDto dto);

        /// <summary>
        /// Genera nuevos tokens usando uno de refresco válido.
        /// </summary>
        Task<TokenResultDto> RefreshTokenAsync(Guid userId, string refreshToken);

        /// <summary>
        /// Registra un usuario nuevo con rol inicial.
        /// </summary>
        Task RegisterAsync(CreateUserDto dto, string roleName);
    }
}
