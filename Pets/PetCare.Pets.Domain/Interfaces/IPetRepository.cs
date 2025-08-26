using PetCare.Pets.Domain.Entities;

namespace PetCare.Pets.Domain.Interfaces
{
    /// <summary>
    /// Define las operaciones de acceso a datos para mascotas.
    /// </summary>
    public interface IPetRepository
    {
        /// <summary>
        /// Obtiene una mascota por su identificador único.
        /// </summary>
        /// <param name="id">Identificador de la mascota.</param>
        /// <returns>La mascota si existe, o null si no se encuentra.</returns>
        Task<Pet?> GetByIdAsync(Guid id);

        /// <summary>
        /// Obtiene una mascota por su ID y el ID del propietario, validando propiedad.
        /// </summary>
        /// <param name="petId">ID de la mascota.</param>
        /// <param name="ownerId">ID del propietario.</param>
        /// <returns>La mascota si pertenece al propietario, o null si no se encuentra o no coincide.</returns>
        Task<Pet?> GetByIdAndOwnerAsync(Guid petId, Guid ownerId);

        /// <summary>
        /// Obtiene todas las mascotas registradas.
        /// </summary>
        /// <returns>Una colección de mascotas.</returns>
        Task<IEnumerable<Pet>> GetAllAsync();

        /// <summary>
        /// Obtiene todas las mascotas asociadas a un propietario específico.
        /// </summary>
        /// <param name="ownerId">ID del propietario.</param>
        /// <returns>Una colección de mascotas pertenecientes al propietario.</returns>
        Task<IEnumerable<Pet>> GetByOwnerIdAsync(Guid ownerId);

        /// <summary>
        /// Crea una nueva mascota.
        /// </summary>
        /// <param name="pet">Entidad de mascota a crear.</param>
        /// <returns>El ID generado para la nueva mascota.</returns>
        Task<Guid> CreateAsync(Pet pet);

        /// <summary>
        /// Actualiza los datos de una mascota existente.
        /// </summary>
        /// <param name="pet">Entidad con los datos actualizados.</param>
        Task UpdateAsync(Pet pet);

        /// <summary>
        /// Elimina una mascota por su identificador.
        /// </summary>
        /// <param name="id">ID de la mascota a eliminar.</param>
        Task DeleteAsync(Guid id);
    }
}
