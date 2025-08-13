namespace PetCare.Booking.Application.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Logging;
    using PetCare.Booking.Application.Interfaces;
    using PetCare.Booking.Domain.DTOs;
    using PetCare.Booking.Domain.Entities;
    using PetCare.Booking.Domain.Interfaces;
    using PetCare.Booking.Extensions;
    using PetCare.Booking.Infrastructure.ExternalServices.Pets.Interfaces;

    public class ReservationService : IReservationService
    {
        private readonly IReservationRepository _repository;
        private readonly IServiceRepository _serviceRepository;
        private readonly IPetServiceClient _petClient;
        private readonly ILogger<ReservationService> _logger;

        public ReservationService(
            IReservationRepository repository,
            IServiceRepository serviceRepository,
            IPetServiceClient petClient,
            ILogger<ReservationService> logger)
        {
            _repository = repository;
            _serviceRepository = serviceRepository;
            _petClient = petClient;
            _logger = logger;
        }

        public async Task<Reservation?> GetByIdAsync(Guid id)
        {
            return await _repository.GetRawByIdAsync(id);
        }

        public async Task<IEnumerable<CreateReservationDto>> GetAllAsync()
        {
            var reservations = await _repository.GetAllAsync();
            return reservations.Select(r => r.ToDto());
        }

        public async Task<Guid> CreateAsync(CreateReservationDto reservationDto)
        {
            _logger.LogInformation("Creando reserva para cliente {ClientId} con mascota {PetId} y servicio {ServiceId}",
                reservationDto.ClientId, reservationDto.PetId, reservationDto.ServiceId);

            // 🔍 Validar servicio
            var service = await _serviceRepository.GetByIdAsync(reservationDto.ServiceId);
            if (service == null)
            {
                _logger.LogWarning("Servicio no encontrado. ServiceId: {ServiceId}", reservationDto.ServiceId);
                throw new InvalidOperationException("El servicio no está disponible.");
            }

            // 🔍 Validar mascota vía cliente HTTP
            var pet = await _petClient.GetByIdAsync(reservationDto.PetId);
            if (pet == null)
            {
                _logger.LogWarning("Mascota no encontrada. PetId: {PetId}", reservationDto.PetId);
                throw new InvalidOperationException("La mascota no existe.");
            }

            if (pet.OwnerId != reservationDto.ClientId)
            {
                _logger.LogWarning("Mascota no pertenece al cliente. PetId: {PetId}, OwnerId: {OwnerId}, ClientId: {ClientId}",
                    pet.Id, pet.OwnerId, reservationDto.ClientId);
                throw new InvalidOperationException("La mascota no pertenece al cliente.");
            }

            // 🔍 Validar conflictos de horario
            bool hasConflict = await _repository.HasConflictAsync(reservationDto.PetId, reservationDto.StartDate, reservationDto.EndDate);
            if (hasConflict)
            {
                _logger.LogWarning("Conflicto de horario detectado para PetId: {PetId}", reservationDto.PetId);
                throw new InvalidOperationException("Ya existe una reserva en ese horario.");
            }

            // ✅ Asignar cuidador desde el servicio
            reservationDto.CaregiverId = service.CaregiverId;

            var reservation = BuildReservationFromDto(reservationDto);

            var created = await _repository.CreateAsync(reservation);
            if (!created)
            {
                _logger.LogError("Error al crear la reserva para PetId: {PetId}", reservationDto.PetId);
                throw new InvalidOperationException("No se pudo crear la reserva.");
            }

            _logger.LogInformation("Reserva creada exitosamente. ReservationId: {ReservationId}", reservation.Id);
            return reservation.Id;
        }

        private Reservation BuildReservationFromDto(CreateReservationDto dto)
        {
            var reservation = Reservation.CreateFromDto(dto);
            reservation.ServiceId = dto.ServiceId;
            reservation.ReservationStatusId = ReservationStatus.Pending.Id;
            reservation.CreatedAt = DateTime.UtcNow;
            return reservation;
        }

        public async Task<bool> CancelAsync(Guid id)
        {
            return await _repository.CancelAsync(id);
        }

        public async Task<bool> AcceptAsync(Guid id)
        {
            return await _repository.AcceptAsync(id);
        }

        public async Task<bool> UpdateNoteAsync(Guid id, string note)
        {
            return await _repository.UpdateNoteAsync(id, note);
        }
    }
}
