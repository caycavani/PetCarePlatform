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

        // ❌ Cancelar reserva
        public async Task<bool> CancelAsync(Guid id)
        {
            var reservation = await GetRawByIdAsync(id);
            if (reservation is null)
                return false;

            reservation.Cancel();
            _context.Reservations.Update(reservation);
            return await _context.SaveChangesAsync() > 0;
        }

        // ✔️ Aceptar reserva
        public async Task<bool> AcceptAsync(Guid id)
        {
            var reservation = await GetRawByIdAsync(id);
            if (reservation is null)
                return false;

            reservation.Accept();
            _context.Reservations.Update(reservation);
            return await _context.SaveChangesAsync() > 0;
        }

        // 📝 Actualizar nota
        public async Task<bool> UpdateNoteAsync(Guid id, string note)
        {
            var reservation = await GetRawByIdAsync(id);
            if (reservation is null)
                return false;

            reservation.UpdateNote(note);
            _context.Reservations.Update(reservation);
            return await _context.SaveChangesAsync() > 0;
        }

        // 🔄 Actualizar estado
        public async Task<bool> UpdateStatusAsync(Guid id, int status)
        {
            var reservation = await GetRawByIdAsync(id);
            if (reservation is null)
                return false;

            reservation.UpdateStatus(status);
            _context.Reservations.Update(reservation);
            return await _context.SaveChangesAsync() > 0;
        }

        // 🧾 Actualizar reserva completa (usado en callbacks de pago)
        public async Task<bool> UpdateAsync(Reservation reservation)
        {
            _context.Reservations.Update(reservation);
            return await _context.SaveChangesAsync() > 0;
        }

        // 🔍 Obtener por ID
        public async Task<Reservation?> GetByIdAsync(Guid id)
        {
            return await GetRawByIdAsync(id);
        }

        // 🔍 Obtener por cliente
        public async Task<IEnumerable<Reservation>> GetByClientIdAsync(Guid clientId)
        {
            return await _context.Reservations
                .Where(r => r.ClientId == clientId)
                .ToListAsync();
        }

        // 🔎 Verificar existencia
        public async Task<bool> ExistsAsync(Guid id)
        {
            return await _context.Reservations.AnyAsync(r => r.Id == id);
        }

        // 🗑️ Eliminar reserva
        public async Task<bool> DeleteAsync(Guid id)
        {
            var reservation = await GetRawByIdAsync(id);
            if (reservation is null)
                return false;

            _context.Reservations.Remove(reservation);
            return await _context.SaveChangesAsync() > 0;
        }

        // ⚠️ Validar conflictos de fechas
        public async Task<bool> HasConflictAsync(Guid petId, DateTime start, DateTime end)
        {
            return await _context.Reservations.AnyAsync(r =>
                r.PetId == petId &&
                r.StartDate < end &&
                r.EndDate > start &&
                r.ReservationStatusId != ReservationStatuses.Canceled
            );
        }
    }
}
