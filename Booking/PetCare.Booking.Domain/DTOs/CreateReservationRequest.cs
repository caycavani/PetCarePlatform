using System.ComponentModel.DataAnnotations;

public class CreateReservationRequest
{
    [Required]
    public Guid PetId { get; set; }

    [Required]
    public Guid ServiceId { get; set; }

    [Required]
    public DateTime StartDate { get; set; }

    [Required]
    public DateTime EndDate { get; set; }

    public string? Note { get; set; }
}
