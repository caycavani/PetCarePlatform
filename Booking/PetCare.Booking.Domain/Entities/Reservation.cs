namespace PetCare.Booking.Domain.Entities
{
    using System;

    public class Reservation
    {
        public Guid Id { get; private set; }
        public Guid PetId { get; private set; }
        public Guid CaregiverId { get; private set; }
        public DateTime StartDate { get; private set; }
        public DateTime EndDate { get; private set; }
        public string Status { get; private set; } = "pending";

        protected Reservation() { }

        public Reservation(Guid petId, Guid caregiverId, DateTime startDate, DateTime endDate)
        {
            Id = Guid.NewGuid();
            PetId = petId;
            CaregiverId = caregiverId;
            StartDate = startDate;
            EndDate = endDate;
        }

        public void Confirm() => Status = "confirmed";
        public void Cancel() => Status = "cancelled";
    }
}
