namespace PetCare.Pets.Domain.Entities
{
    public class Pet
    {
        public Guid Id { get;  set; }

        public Guid OwnerId { get;  set; }

        public string Name { get;  set; }

        public string Type { get;  set; }

        public DateTime BirthDate { get;  set; }
        public string Breed { get; set; }
        public int Age { get; set; }


        public Pet() { }

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
