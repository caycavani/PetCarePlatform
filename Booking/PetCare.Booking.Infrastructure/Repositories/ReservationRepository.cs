using Microsoft.EntityFrameworkCore;
using PetCare.Booking.Domain.Entities;
using PetCare.Booking.Domain.Interfaces;
using PetCare.Booking.Infrastructure.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PetCare.Booking.Infrastructure.Repositories
{
    public class ReservationRepository : IReservationRepository
    {
        private readonly ReservationDbContext _context;

        public ReservationRepository(ReservationDbContext context)
        {
            _context = context;
        }

        // 🧠 Devuelve entidad completa para validación en controlador
        public async Task<Reservation?> GetRawByIdAsync(Guid id)
        {
            return await _context.Reservations.FindAsync(id);
        }

        // 📋 Retorna todas las reservas
        public async Task<IEnumerable<Reservation>> GetAllAsync()
        {
            return await _context.Reservations.ToListAsync();
        }

        // 🆕 Crear una nueva reserva
        public async Task<bool> CreateAsync(Reservation reservation)
        {
            await _context.Reservations.AddAsync(reservation);
            return await _context.SaveChangesAsync() > 0;
        }

        // ❌ Cancelar reserva (marcar como cancelada si tienes columna correspondiente)
        public async Task<bool> CancelAsync(Guid id)
        {
            var reservation = await GetRawByIdAsync(id);
            if (reservation is null)
                return false;

            reservation.Cancel();
            _context.Reservations.Update(reservation);
            return await _context.SaveChangesAsync() > 0;
        }

        // ✔️ Aceptar reserva (marca como aceptada si tienes lógica de estado)
        public async Task<bool> AcceptAsync(Guid id)
        {
            var reservation = await GetRawByIdAsync(id);
            if (reservation is null)
                return false;

            reservation.Accept();
            _context.Reservations.Update(reservation);
            return await _context.SaveChangesAsync() > 0;
        }

        // 📝 Actualizar nota (sin validar propiedad aquí)
        public async Task<bool> UpdateNoteAsync(Guid id, string note)
        {
            var reservation = await GetRawByIdAsync(id);
            if (reservation is null)
                return false;

            reservation.UpdateNote(note); // Método sugerido en entidad
            _context.Reservations.Update(reservation);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<Reservation?> GetByIdAsync(Guid id)
        {
            return await GetRawByIdAsync(id); // Ya lo tienes definido, lo reutilizamos
        }

        public async Task<IEnumerable<Reservation>> GetByClientIdAsync(Guid clientId)
        {
            return await _context.Reservations
                .Where(r => r.ClientId == clientId)
                .ToListAsync();
        }

        public async Task<bool> ExistsAsync(Guid id)
        {
            return await _context.Reservations.AnyAsync(r => r.Id == id);
        }

        public async Task<bool> UpdateStatusAsync(Guid id, int status)
        {
            var reservation = await GetRawByIdAsync(id);
            if (reservation is null)
                return false;

            reservation.UpdateStatus(status);
            _context.Reservations.Update(reservation);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var reservation = await GetRawByIdAsync(id);
            if (reservation is null)
                return false;

            _context.Reservations.Remove(reservation);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> HasConflictAsync(Guid petId, DateTime start, DateTime end)
        {
            return await _context.Reservations.AnyAsync(r =>
                r.PetId == petId &&
                r.StartDate < end &&
                r.EndDate > start &&
                r.ReservationStatusId != 3 // excluye reservas canceladas
            );
        }

    }
}
