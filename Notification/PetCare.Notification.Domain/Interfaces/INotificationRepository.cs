using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PetCare.Notification.Domain.Entities;

namespace PetCare.Notification.Domain.Interfaces
{
    /// <summary>
    /// Contrato de persistencia para operaciones sobre la entidad Nitificacion.
    /// </summary>
    public interface INitificacionRepository
    {
        /// <summary>
        /// Persiste una nueva nitificación en el sistema.
        /// </summary>
        Task AddAsync(Ntification nitificacion);

        /// <summary>
        /// Obtiene una nitificación por su identificador único.
        /// </summary>
        Task<Ntification?> GetByIdAsync(Guid id);

        /// <summary>
        /// Obtiene todas las nitificaciones asociadas al correo del destinatario.
        /// </summary>
        Task<IEnumerable<Ntification>> GetByRecipientEmailAsync(string email);

        /// <summary>
        /// Obtiene todas las nitificaciones registradas en el sistema.
        /// </summary>
        Task<IEnumerable<Ntification>> GetAllAsync();

        /// <summary>
        /// Elimina una nitificación por su identificador.
        /// </summary>
        Task DeleteAsync(Guid id);
    }
}
