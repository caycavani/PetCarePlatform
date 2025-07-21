using System.ComponentModel.DataAnnotations;

namespace PetCare.Auth.Api.Models
{
    public class CreateRoleRequest
    {
        [Required(ErrorMessage = "El nombre del rol es obligatorio.")]
        [MaxLength(50, ErrorMessage = "El nombre del rol no puede superar los 50 caracteres.")]
        public string Name { get; set; } = string.Empty;

        [MaxLength(200, ErrorMessage = "La descripción no puede superar los 200 caracteres.")]
        public string Description { get; set; } = string.Empty;

        public CreateRoleRequest() { }

        public CreateRoleRequest(string name, string description)
        {
            Name = name;
            Description = description;
        }
    }
}
