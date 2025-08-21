using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PetCare.Notification.Domain.Entities;
using PetCare.Notification.Domain.Interfaces;
using PetCare.Notification.Infrastructure.Persistence;

namespace PetCare.Notification.Infrastructure.Repositories
{
    /// <summary>
    /// Repositorio para operaciones de persistencia sobre la entidad Nitificacion.
    /// </summary>
    public class NitificacionRepository : INitificacionRepository
    {
        private readonly NotificationDbContext _context;

        public NitificacionRepository(NotificationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Agrega una nueva nitificación a la base de datos.
        /// </summary>
        public async Task AddAsync(Ntification nitificacion)
        {
            await _context.Notifications.AddAsync(nitificacion);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Obtiene una nitificación por su identificador único.
        /// </summary>
        public async Task<Ntification?> GetByIdAsync(Guid id)
        {
            return await _context.Notifications
                .FirstOrDefaultAsync(n => n.Id == id);
        }

        /// <summary>
        /// Obtiene todas las nitificaciones asociadas al correo del destinatario.
        /// </summary>
        public async Task<IEnumerable<Ntification>> GetByRecipientEmailAsync(string email)
        {
            return await _context.Notifications
                .Where(n => n.Recipient.Email == email)
                .ToListAsync();
        }

        /// <summary>
        /// Obtiene todas las nitificaciones registradas en el sistema.
        /// </summary>
        public async Task<IEnumerable<Ntification>> GetAllAsync()
        {
            return await _context.Notifications
                .OrderByDescending(n => n.ScheduledAt)
                .ToListAsync();
        }

        /// <summary>
        /// Elimina una nitificación por su identificador.
        /// </summary>
        public async Task DeleteAsync(Guid id)
        {
            var nitificacion = await _context.Notifications.FindAsync(id);
            if (nitificacion is null) return;

            _context.Notifications.Remove(nitificacion);
            await _context.SaveChangesAsync();
        }
    }
}
