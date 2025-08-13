using PetCare.Booking.Domain.DTOs;
using PetCare.Booking.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PetCare.Booking.Application.Interfaces
{
    public interface IReservationService
    {
        // 🔍 Obtener una reserva por ID (entidad completa)
        Task<Reservation?> GetByIdAsync(Guid id);

        // 📋 Listar todas las reservas (DTO para vista)
        Task<IEnumerable<CreateReservationDto>> GetAllAsync();

        // 🆕 Crear una nueva reserva
        Task<Guid> CreateAsync(CreateReservationDto reservationDto);

        // ❌ Cancelar una reserva
        Task<bool> CancelAsync(Guid id);

        // ✔️ Aceptar una reserva
        Task<bool> AcceptAsync(Guid id);

        // 📝 Actualizar nota de la reserva
        Task<bool> UpdateNoteAsync(Guid id, string note);
    }
}
