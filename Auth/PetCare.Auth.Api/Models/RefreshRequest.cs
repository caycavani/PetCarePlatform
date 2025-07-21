using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PetCare.Auth.Api.Models
{
    public class RefreshRequest : IValidatableObject
    {
        [Required(ErrorMessage = "El campo UserId es obligatorio.")]
        public Guid UserId { get; set; }

        [Required(ErrorMessage = "El token de refresco es obligatorio.")]
        [StringLength(200, MinimumLength = 20, ErrorMessage = "El token debe tener al menos 20 caracteres.")]
        public string RefreshToken { get; set; } = string.Empty;

        public RefreshRequest() { }

        public RefreshRequest(Guid userId, string refreshToken)
        {
            UserId = userId;
            RefreshToken = refreshToken;
        }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (UserId == Guid.Empty)
            {
                yield return new ValidationResult("El UserId no puede estar vacío.", new[] { nameof(UserId) });
            }
        }
    }
}
