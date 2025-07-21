using System.Threading.Tasks;


namespace PetCare.Booking.Api.Services
{
    public class AuthValidatorService
    {
        private readonly HttpClient _client;

        public AuthValidatorService(HttpClient client)
        {
            _client = client;
        }

        public async Task<bool> IsAuthApiHealthyAsync()
        {
            var response = await _client.GetAsync("/health");
            return response.IsSuccessStatusCode;
        }
    }
}
