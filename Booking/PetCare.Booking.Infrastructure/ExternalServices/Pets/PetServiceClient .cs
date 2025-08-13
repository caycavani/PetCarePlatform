using PetCare.Booking.Infrastructure.ExternalServices.Pets.Interfaces;
using PetCare.Shared.DTOs;
using System.Net.Http.Json;

namespace PetCare.Booking.Infrastructure.ExternalServices.Pets
{
    public class PetServiceClient : IPetServiceClient
    {
        private readonly HttpClient _httpClient;

        public PetServiceClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<PetDto?> GetByIdAsync(Guid petId)
        {
            var response = await _httpClient.GetAsync($"/api/pets/{petId}");
            if (!response.IsSuccessStatusCode)
                return null;

            return await response.Content.ReadFromJsonAsync<PetDto>();
        }

        public async Task<IEnumerable<PetDto>> GetAllAsync()
        {
            var response = await _httpClient.GetAsync("/api/pets");
            if (!response.IsSuccessStatusCode)
                return Enumerable.Empty<PetDto>();

            return await response.Content.ReadFromJsonAsync<IEnumerable<PetDto>>() ?? Enumerable.Empty<PetDto>();
        }

        public async Task<PetDto?> GetByIdAndOwnerAsync(Guid petId, Guid ownerId)
        {
            var pet = await GetByIdAsync(petId);
            return pet != null && pet.OwnerId == ownerId ? pet : null;
        }
    }
}
