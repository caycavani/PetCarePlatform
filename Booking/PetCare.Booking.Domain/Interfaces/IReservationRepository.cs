using PetCare.Booking.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PetCare.Booking.Domain.Interfaces
{
    public interface IReservationRepository
    {
        Task<Reservation?> GetRawByIdAsync(Guid id);              // 🛡️ Devuelve la entidad completa
        Task<Reservation?> GetByIdAsync(Guid id);                 // Alias para compatibilidad
        Task<IEnumerable<Reservation>> GetAllAsync();             // 📋 Todas las reservas
        Task<IEnumerable<Reservation>> GetByClientIdAsync(Guid clientId); // 🔎 Reservas por cliente
        Task<bool> ExistsAsync(Guid id);                          // ❓ Verifica existencia
        Task<bool> CreateAsync(Reservation reservation);          // 🆕 Crear reserva
        Task<bool> CancelAsync(Guid id);                          // ❌ Cancelar (lógico)
        Task<bool> AcceptAsync(Guid id);                          // ✔️ Aceptar
        Task<bool> UpdateNoteAsync(Guid id, string note);         // 📝 Actualizar nota
        Task<bool> UpdateStatusAsync(Guid id, int status);        // 🔄 Actualizar estado
        Task<bool> DeleteAsync(Guid id);                          // 🗑️ Borrado físico

        Task<bool> HasConflictAsync(Guid petId, DateTime start, DateTime end);

    }
}
