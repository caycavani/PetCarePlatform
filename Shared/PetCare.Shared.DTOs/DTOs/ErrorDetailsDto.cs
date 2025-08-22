namespace PetCare.Shared.DTOs.DTOs
{
    public class ErrorDetailsDto
    {
        public string Message { get; set; }
        public string Source { get; set; }
        public string ExceptionType { get; set; }
        public string StackTrace { get; set; }
    }
}
