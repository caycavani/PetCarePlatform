using System.ComponentModel.DataAnnotations;

public class LoginUserDto
{
    [Required(ErrorMessage = "El usuario o correo es obligatorio.")]
    [StringLength(100, ErrorMessage = "El valor no debe exceder los 100 caracteres.")]
    public string Identifier { get; set; } = string.Empty; // puede ser email o username

    [Required(ErrorMessage = "La contraseña es obligatoria.")]
    [StringLength(100, MinimumLength = 6, ErrorMessage = "Debe tener entre 6 y 100 caracteres.")]
    public string Password { get; set; } = string.Empty;
}
