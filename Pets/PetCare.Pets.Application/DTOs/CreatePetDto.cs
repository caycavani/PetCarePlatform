using System.ComponentModel.DataAnnotations;

namespace PetCare.Pets.Application.DTOs
{
    public class CreatePetDto
    {
        /// <summary>Identificador del dueño de la mascota.</summary>
        [Required(ErrorMessage = "El campo OwnerId es obligatorio.")]
        public Guid OwnerId { get; set; }

        /// <summary>Nombre de la mascota.</summary>
        [Required(ErrorMessage = "El campo Name es obligatorio.")]
        [MaxLength(100, ErrorMessage = "El nombre no puede exceder los 100 caracteres.")]
        public string Name { get; set; } = string.Empty;

        /// <summary>Tipo de mascota (ej. Dog, Cat).</summary>
        [Required(ErrorMessage = "El campo Type es obligatorio.")]
        [MaxLength(50, ErrorMessage = "El tipo no puede exceder los 50 caracteres.")]
        public string Type { get; set; } = string.Empty;

        /// <summary>Fecha de nacimiento de la mascota.</summary>
        [Required(ErrorMessage = "El campo BirthDate es obligatorio.")]
        public DateTime BirthDate { get; set; }

        /// <summary>Raza de la mascota.</summary>
        [MaxLength(100, ErrorMessage = "La raza no puede exceder los 100 caracteres.")]
        public string Breed { get; set; } = string.Empty;

        /// <summary>Edad calculada o declarada de la mascota.</summary>
        [Range(0, 100, ErrorMessage = "La edad debe estar entre 0 y 100.")]
        public int Age { get; set; }
    }
}
