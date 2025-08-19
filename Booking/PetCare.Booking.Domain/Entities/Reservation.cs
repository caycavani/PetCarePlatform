using PetCare.Booking.Domain.DTOs;
using System;
using System.ComponentModel.DataAnnotations;

namespace PetCare.Booking.Domain.Entities
{
    public class Reservation
    {
        public Guid Id { get; set; }
        public Guid PetId { get; set; }
        public Guid CaregiverId { get; set; }
        public Guid ClientId { get; set; }
        public Guid ServiceId { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public int ReservationStatusId { get; set; } // Usa enum si lo prefieres
        public ReservationStatus Status { get; set; }

        [StringLength(500)]
        public string? Note { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        // 🧾 Propiedades de trazabilidad de pago
        [StringLength(100)]
        public string? PaymentStatus { get; private set; }

        [StringLength(100)]
        public string? TransactionId { get; private set; }

        public DateTime? PaymentConfirmedAt { get; private set; }

        // 🔗 Navegación
        public Service? Service { get; set; }

        // 👷 Constructor vacío para EF Core
        public Reservation() { }

        // 👷 Constructor completo
        public Reservation(Guid id, Guid clientId, Guid caregiverId, Guid petId, DateTime startDate, DateTime endDate, string? note)
        {
            Id = id;
            ClientId = clientId;
            CaregiverId = caregiverId;
            PetId = petId;
            StartDate = startDate;
            EndDate = endDate;
            Note = note;
        }

        // 🧠 Métodos de dominio
        public void AssignClient(Guid clientId)
        {
            ClientId = clientId;
            UpdatedAt = DateTime.UtcNow;
        }

        public void UpdateStatus(int statusId)
        {
            ReservationStatusId = statusId;
            UpdatedAt = DateTime.UtcNow;
        }

        public void UpdateNote(string note)
        {
            Note = note;
            UpdatedAt = DateTime.UtcNow;
        }

        public void Cancel()
        {
            ReservationStatusId = ReservationStatuses.Canceled;
            UpdatedAt = DateTime.UtcNow;
        }

        public void Accept()
        {
            ReservationStatusId = ReservationStatuses.Accepted;
            UpdatedAt = DateTime.UtcNow;
        }

        public void MarkAsPaid(string transactionId, DateTime timestamp)
        {
            PaymentStatus = "Paid";
            TransactionId = transactionId;
            PaymentConfirmedAt = timestamp;
            ReservationStatusId = ReservationStatuses.Confirmed;
            UpdatedAt = DateTime.UtcNow;
        }

        public void MarkAsPaymentFailed(string transactionId, DateTime timestamp)
        {
            PaymentStatus = "Failed";
            TransactionId = transactionId;
            PaymentConfirmedAt = timestamp;
            ReservationStatusId = ReservationStatuses.Rejected;
            UpdatedAt = DateTime.UtcNow;
        }

        public void MarkAsRefunded(string transactionId, DateTime timestamp)
        {
            PaymentStatus = "Refunded";
            TransactionId = transactionId;
            PaymentConfirmedAt = timestamp;
            ReservationStatusId = ReservationStatuses.Refunded;
            UpdatedAt = DateTime.UtcNow;
        }

        // 🎯 Método de fábrica consolidado
        public static Reservation CreateFromDto(CreateReservationDto dto)
        {
            return new Reservation(
                Guid.NewGuid(),
                dto.ClientId,
                dto.CaregiverId,
                dto.PetId,
                dto.StartDate,
                dto.EndDate,
                dto.Note
            )
            {
                ReservationStatusId = ReservationStatuses.Pending,
                ServiceId = dto.ServiceId,
                CreatedAt = DateTime.UtcNow
            };
        }
    }

    // 📌 Estados de la reserva
    public static class ReservationStatuses
    {
        public const int Pending = 0;
        public const int Accepted = 1;
        public const int Canceled = 2;
        public const int Confirmed = 3;
        public const int Rejected = 4;
        public const int Refunded = 5;
        // Puedes extender con otros estados como Expired, InProgress, etc.
    }
}
