using System.ComponentModel.DataAnnotations;

namespace PetCare.Auth.Api.Models
{
    public class CreateRoleRequest
    {
        [Required(ErrorMessage = "El nombre del rol es obligatorio.")]
        [StringLength(50, ErrorMessage = "El nombre no puede tener más de 50 caracteres.")]
        public string Name { get; set; }

     
    }
}
