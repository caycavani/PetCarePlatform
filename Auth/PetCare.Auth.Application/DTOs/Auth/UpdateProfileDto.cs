using System.ComponentModel.DataAnnotations;

public class UpdateProfileDto
{
    [Required]
    [StringLength(100, ErrorMessage = "El nombre completo no puede superar los 100 caracteres.")]
    public string FullName { get; set; } = string.Empty;

    [Phone(ErrorMessage = "El número telefónico no es válido.")]
    [StringLength(20, ErrorMessage = "El número telefónico no puede superar los 20 caracteres.")]
    public string Phone { get; set; } = string.Empty;
}
