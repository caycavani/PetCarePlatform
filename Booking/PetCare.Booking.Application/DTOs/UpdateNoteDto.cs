using System.ComponentModel.DataAnnotations;

namespace PetCare.Booking.Application.DTOs;

public class UpdateNoteDto
{
    [Required(ErrorMessage = "La nota es obligatoria.")]
    [MinLength(1, ErrorMessage = "La nota no puede estar vacía.")]
    public string Note { get; set; } = string.Empty;
}
