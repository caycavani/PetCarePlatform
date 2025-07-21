using System.ComponentModel.DataAnnotations;

namespace PetCare.Auth.Api.Models
{
    public class LoginRequest
    {
        [Required(ErrorMessage = "El campo Email es obligatorio.")]
        [EmailAddress(ErrorMessage = "El email no tiene un formato válido.")]
        [MaxLength(100, ErrorMessage = "El email no puede superar los 100 caracteres.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "La contraseña es obligatoria.")]
        [StringLength(100, MinimumLength = 8, ErrorMessage = "La contraseña debe tener entre 8 y 100 caracteres.")]
        public string Password { get; set; } = string.Empty;

        public LoginRequest() { }

        public LoginRequest(string email, string password)
        {
            Email = email;
            Password = password;
        }
    }
}
