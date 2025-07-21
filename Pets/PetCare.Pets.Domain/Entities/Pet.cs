namespace PetCare.Pets.Domain.Entities
{
    public class Pet
    {
        public Guid Id { get; private set; }

        public Guid OwnerId { get; private set; }

        public string Name { get; private set; }

        public string Type { get; private set; }

        public DateTime BirthDate { get; private set; }

        protected Pet() { }

        public Pet(Guid ownerId, string name, string type, DateTime birthDate)
        {
            Id = Guid.NewGuid();
            OwnerId = ownerId;
            Name = name;
            Type = type;
            BirthDate = birthDate;
        }
    }
}
