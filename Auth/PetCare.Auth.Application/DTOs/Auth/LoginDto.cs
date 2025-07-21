using System.ComponentModel.DataAnnotations;

namespace PetCare.Auth.Application.DTOs.Auth
{
    public class LoginUserDto
    {
        [Required(ErrorMessage = "El correo es obligatorio.")]
        [EmailAddress(ErrorMessage = "Formato de correo inválido.")]
        [StringLength(100, ErrorMessage = "El correo no debe exceder los 100 caracteres.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "La contraseña es obligatoria.")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Debe tener entre 6 y 100 caracteres.")]
        public string Password { get; set; } = string.Empty;
    }
}
