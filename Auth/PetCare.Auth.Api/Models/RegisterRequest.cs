using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using PetCare.Auth.Domain.Interfaces;

namespace PetCare.Auth.Api.Models
{
    public class RegisterRequest : IValidatableObject
    {
        [Required(ErrorMessage = "El campo Email es obligatorio.")]
        [EmailAddress(ErrorMessage = "El email no tiene un formato v�lido.")]
        [MaxLength(100, ErrorMessage = "El email no puede superar los 100 caracteres.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "La contrase�a es obligatoria.")]
        [StringLength(100, MinimumLength = 8, ErrorMessage = "La contrase�a debe tener entre 8 y 100 caracteres.")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&\.\-]).{8,}$",
            ErrorMessage = "Debe contener al menos una may�scula, una min�scula, un n�mero y un car�cter especial.")]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "El nombre completo es obligatorio.")]
        [MaxLength(100, ErrorMessage = "El nombre completo no puede superar los 100 caracteres.")]
        public string FullName { get; set; } = string.Empty;

        [Required(ErrorMessage = "El n�mero de tel�fono es obligatorio.")]
        [Phone(ErrorMessage = "El n�mero de tel�fono no tiene un formato v�lido.")]
        [MaxLength(20, ErrorMessage = "El tel�fono no puede superar los 20 caracteres.")]
        public string Phone { get; set; } = string.Empty;

        [Required(ErrorMessage = "El rol es obligatorio.")]
        public string Role { get; set; } = "user";

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var roleRepo = validationContext.GetService(typeof(IRoleRepository)) as IRoleRepository;

            if (roleRepo is null)
            {
                yield return new ValidationResult(
                    "No se pudo acceder a IRoleRepository para validar el rol.",
                    new[] { nameof(Role) });
                yield break;
            }

            var roleTask = roleRepo.GetByNameAsync(Role.ToLower());
            roleTask.Wait();

            if (roleTask.Result is null)
            {
                yield return new ValidationResult("El rol indicado no existe.", new[] { nameof(Role) });
            }
        }
    }
}
