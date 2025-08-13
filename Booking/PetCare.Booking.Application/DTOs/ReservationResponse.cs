public class ReservationResponse
{
    public Guid Id { get; set; }
    public Guid PetId { get; set; }
    public Guid ClientId { get; set; }
    public Guid ServiceId { get; set; }

    public Guid CaregiverId { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int ReservationStatusId { get; set; }
    public string Note { get; set; } = string.Empty;

  
}