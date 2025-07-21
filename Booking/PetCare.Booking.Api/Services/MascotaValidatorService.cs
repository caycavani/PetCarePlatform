using System.Threading.Tasks;



namespace PetCare.Booking.Api.Services
{
    public class MascotaValidatorService
    {
        private readonly HttpClient _httpClient;

        public MascotaValidatorService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<bool> ValidarMascotaExiste(int mascotaId)
        {
            var response = await _httpClient.GetAsync($"/api/mascotas/{mascotaId}");
            return response.IsSuccessStatusCode;
        }
    }
}
