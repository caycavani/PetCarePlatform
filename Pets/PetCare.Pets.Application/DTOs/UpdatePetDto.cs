namespace PetCare.Pets.Application.DTOs
{
    public class UpdatePetDto
    {
        public Guid OwnerId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Breed { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public DateTime BirthDate { get; set; }
        public int Age { get; set; }

    }
}
