using System.ComponentModel.DataAnnotations;

namespace PetCare.Auth.Application.DTOs.User
{
    public class CreateUserDto
    {
        [Required(ErrorMessage = "El correo es obligatorio.")]
        [EmailAddress(ErrorMessage = "El correo no tiene un formato válido.")]
        [StringLength(100, ErrorMessage = "El correo no debe exceder los 100 caracteres.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "La contraseña es obligatoria.")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "La contraseña debe tener entre 6 y 100 caracteres.")]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "El nombre completo es obligatorio.")]
        [StringLength(100, ErrorMessage = "El nombre completo no debe exceder los 100 caracteres.")]
        public string FullName { get; set; } = string.Empty;

        [Required(ErrorMessage = "El teléfono es obligatorio.")]
        [Phone(ErrorMessage = "El formato del teléfono no es válido.")]
        [StringLength(20, ErrorMessage = "El teléfono no debe exceder los 20 caracteres.")]
        public string Phone { get; set; } = string.Empty;
    }
}
