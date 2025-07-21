using PetCare.Auth.Domain.Entities;
using System;
using System.Threading.Tasks;

namespace PetCare.Auth.Domain.Interfaces
{
    public interface IRefreshTokenRepository
    {
        /// <summary>
        /// Agrega un nuevo token de refresco para un usuario.
        /// </summary>
        Task AddAsync(RefreshToken token);

        /// <summary>
        /// Obtiene un token de refresco específico por usuario y valor de token.
        /// </summary>
        Task<RefreshToken?> GetByUserAndTokenAsync(Guid userId, string token);

        /// <summary>
        /// Actualiza un token de refresco existente (por ejemplo, al rotarlo).
        /// </summary>
        Task UpdateAsync(RefreshToken token);
    }
}
