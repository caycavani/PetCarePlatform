using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PetCare.Auth.Domain.Entities;

namespace PetCare.Auth.Domain.Interfaces
{
    public interface IRoleRepository
    {
        /// <summary>
        /// Obtiene todos los roles registrados.
        /// </summary>
        Task<List<Role>> GetAllAsync();

        /// <summary>
        /// Busca un rol por su identificador único.
        /// </summary>
        Task<Role?> GetByIdAsync(Guid id);

        /// <summary>
        /// Busca un rol por su nombre (no normalizado).
        /// </summary>
        Task<Role?> GetByNameAsync(string name);

        /// <summary>
        /// Busca un rol por su nombre normalizado.
        /// </summary>
        Task<Role?> GetByNormalizedNameAsync(string normalizedName);

        /// <summary>
        /// Verifica si existe un rol por nombre (normal o normalizado).
        /// </summary>
        Task<bool> ExistsAsync(string name);

        /// <summary>
        /// Agrega un nuevo rol al sistema.
        /// </summary>
        Task AddAsync(Role role);

        /// <summary>
        /// Actualiza un rol existente.
        /// </summary>
        Task UpdateAsync(Role role);

        /// <summary>
        /// Elimina un rol por su identificador.
        /// </summary>
        Task DeleteAsync(Guid id);
    }
}
