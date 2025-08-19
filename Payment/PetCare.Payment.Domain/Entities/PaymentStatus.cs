namespace PetCare.Payment.Domain.Entities
{
    public class PaymentStatus
    {
        public int Id { get; private set; }
        public string Name { get; private set; }

        protected PaymentStatus() { }

        public PaymentStatus(int id, string name)
        {
            Id = id;
            Name = name;
        }

        public override string ToString() => Name;
    }
}
