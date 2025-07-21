using System.ComponentModel.DataAnnotations;

namespace PetCare.Pets.Application.DTOs
{
    public class CreatePetDto
    {
        [Required]
        public Guid OwnerId { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string Type { get; set; } = string.Empty;

        [Required]
        public DateTime BirthDate { get; set; }
    }
}
