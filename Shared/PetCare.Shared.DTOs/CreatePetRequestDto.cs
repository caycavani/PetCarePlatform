using System.ComponentModel.DataAnnotations;

namespace PetCare.Shared.DTOs;

public class CreatePetRequestDto
{
    [Required(ErrorMessage = "El nombre es obligatorio.")]
    [StringLength(50, ErrorMessage = "El nombre no puede superar los 50 caracteres.")]
    public string Name { get; set; } = null!;

    [Required(ErrorMessage = "La raza es obligatoria.")]
    [StringLength(50, ErrorMessage = "La raza no puede superar los 50 caracteres.")]
    public string Breed { get; set; } = null!;

    [Required(ErrorMessage = "El género es obligatorio.")]
    [RegularExpression("^(Male|Female)$", ErrorMessage = "El género debe ser 'Male' o 'Female'.")]
    public string Gender { get; set; } = null!;

    [Required(ErrorMessage = "La fecha de nacimiento es obligatoria.")]
    [DataType(DataType.Date)]
    [CustomValidation(typeof(CreatePetRequestDto), nameof(ValidateBirthDate))]
    public DateTime BirthDate { get; set; }

    // 🎯 Validación: no permitir fechas futuras
    public static ValidationResult? ValidateBirthDate(DateTime date, ValidationContext context)
    {
        return date > DateTime.UtcNow
            ? new ValidationResult("La fecha de nacimiento no puede ser futura.")
            : ValidationResult.Success;
    }
}
