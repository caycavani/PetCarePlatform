namespace PetCare.Payment.Domain.Entities
{
    using System;

    public class Pay
    {
        public Guid Id { get; private set; }
        public Guid ReservationId { get; private set; }
        public decimal Amount { get; private set; }
        public string Method { get; private set; }
        public DateTime PaidAt { get; private set; }

        protected Pay() { }

        public Pay(Guid reservationId, decimal amount, string method)
        {
            Id = Guid.NewGuid();
            ReservationId = reservationId;
            Amount = amount;
            Method = method;
            PaidAt = DateTime.UtcNow;
        }
    }
}
