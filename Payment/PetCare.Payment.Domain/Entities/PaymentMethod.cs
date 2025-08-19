namespace PetCare.Payment.Domain.Entities
{
    public class PaymentMethod
    {
        public int Id { get; private set; }
        public string Name { get; private set; }

        protected PaymentMethod() { }

        public PaymentMethod(int id, string name)
        {
            Id = id;
            Name = name;
        }

        public override string ToString() => Name;
    }
}
