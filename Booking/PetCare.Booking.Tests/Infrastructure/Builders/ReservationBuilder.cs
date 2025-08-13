using System;
using PetCare.Booking.Domain.Entities;

namespace PetCare.Booking.Tests.Builders
{
    public class ReservationBuilder
    {
        // 🧱 Campos internos con valores por defecto seguros
        private Guid _id = Guid.NewGuid();
        private Guid _clientId = Guid.NewGuid();
        private Guid _caregiverId = Guid.NewGuid();
        private Guid _petId = Guid.NewGuid();
        private DateTime _startDate = DateTime.UtcNow.Date.AddDays(2);
        private DateTime _endDate = DateTime.UtcNow.Date.AddDays(5);
        private int _statusId = 2;
        private string _note = "Reserva generada desde builder";

        // 🔧 Métodos fluidos para personalización
        public ReservationBuilder WithId(Guid id)
        {
            _id = id;
            return this;
        }

        public ReservationBuilder WithClientId(Guid clientId)
        {
            _clientId = clientId;
            return this;
        }

        public ReservationBuilder WithClient(Guid clientId)
        {
            _clientId = clientId;
            return this;
        }

        public ReservationBuilder WithCaregiver(Guid caregiverId)
        {
            _caregiverId = caregiverId;
            return this;
        }

        public ReservationBuilder WithPet(Guid petId)
        {
            _petId = petId;
            return this;
        }

        public ReservationBuilder WithDates(DateTime start, DateTime end)
        {
            _startDate = start;
            _endDate = end;
            return this;
        }

        public ReservationBuilder WithStatus(int statusId)
        {
            _statusId = statusId;
            return this;
        }

        public ReservationBuilder WithNote(string note)
        {
            _note = string.IsNullOrWhiteSpace(note) ? "Nota generada por builder" : note;
            return this;
        }

        // 🧬 Construcción final
        public Reservation Build()
        {
            var reservation = new Reservation(
                _id,
                _clientId,
                _caregiverId,
                _petId,
                _startDate,
                _endDate,
                _note
            );

            reservation.ReservationStatusId = _statusId;
            return reservation;
        }

    }
}
