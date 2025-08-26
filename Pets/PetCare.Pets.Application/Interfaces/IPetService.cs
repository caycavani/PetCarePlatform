using PetCare.Pets.Application.DTOs;
using PetCare.Shared.DTOs;

namespace PetCare.Pets.Application.Interfaces
{
    /// <summary>
    /// Define las operaciones de negocio para el manejo de mascotas.
    /// </summary>
    public interface IPetService
    {
        /// <summary>
        /// Obtiene todas las mascotas registradas.
        /// </summary>
        /// <returns>Una colección de <see cref="PetDto"/>.</returns>
        Task<IEnumerable<PetDto>> GetAllAsync();

        /// <summary>
        /// Obtiene todas las mascotas asociadas a un propietario específico.
        /// </summary>
        /// <param name="ownerId">Identificador del propietario.</param>
        /// <returns>Una colección de <see cref="PetDto"/> pertenecientes al propietario.</returns>
        Task<IEnumerable<PetDto>> GetAllByOwnerAsync(Guid ownerId);

        /// <summary>
        /// Obtiene una mascota por su identificador único.
        /// </summary>
        /// <param name="id">Identificador de la mascota.</param>
        /// <returns>Un <see cref="PetDto"/> si existe, o null si no se encuentra.</returns>
        Task<PetDto?> GetByIdAsync(Guid id);

        /// <summary>
        /// Crea una nueva mascota.
        /// </summary>
        /// <param name="dto">Datos de la mascota a crear.</param>
        /// <returns>El identificador único de la mascota creada.</returns>
        Task<Guid> CreateAsync(CreatePetDto dto);

        /// <summary>
        /// Actualiza los datos de una mascota existente.
        /// </summary>
        /// <param name="id">Identificador de la mascota.</param>
        /// <param name="dto">Datos actualizados.</param>
        /// <returns>True si la actualización fue exitosa, false si no se encontró la mascota.</returns>
        Task<bool> UpdateAsync(Guid id, UpdatePetDto dto);

        /// <summary>
        /// Elimina una mascota por su identificador.
        /// </summary>
        /// <param name="id">Identificador de la mascota.</param>
        /// <returns>True si la eliminación fue exitosa, false si no se encontró la mascota.</returns>
        Task<bool> DeleteAsync(Guid id);
    }
}
