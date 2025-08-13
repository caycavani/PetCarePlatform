using PetCare.Pets.Application.DTOs;
using PetCare.Pets.Application.Interfaces;
using PetCare.Pets.Domain.Entities;
using PetCare.Pets.Domain.Interfaces;
using PetCare.Shared.DTOs;

namespace PetCare.Pets.Application.Services
{
    public class PetService : IPetService
    {
        private readonly IPetRepository _repository;

        public PetService(IPetRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<PetDto>> GetAllAsync()
        {
            var pets = await _repository.GetAllAsync();
            return pets.Select(p => new PetDto
            {
                Id = p.Id,
                Name = p.Name,
                Breed = p.Breed,
                Age = p.Age,
                OwnerId = p.OwnerId
            });
        }

        public async Task<PetDto?> GetByIdAsync(Guid id)
        {
            var pet = await _repository.GetByIdAsync(id);
            if (pet is null) return null;

            return new PetDto
            {
                Id = pet.Id,
                Name = pet.Name,
                Breed = pet.Breed,
                Age = pet.Age,
                OwnerId = pet.OwnerId
            };
        }

        public async Task<Guid> CreateAsync(CreatePetDto dto)
        {
            var pet = new Pet
            {
                Id = Guid.NewGuid(),
                Name = dto.Name,
                Breed = dto.Breed,
                Age = dto.Age,
                OwnerId = dto.OwnerId,
                Type = dto.Type // ✅ Este campo debe estar presente
            };

            await _repository.CreateAsync(pet);
            return pet.Id;
        }

        public async Task<bool> UpdateAsync(Guid id, UpdatePetDto dto)
        {
            var existingPet = await _repository.GetByIdAsync(id);
            if (existingPet is null) return false;

            existingPet.Name = dto.Name;
            existingPet.Breed = dto.Breed;
            existingPet.Age = dto.Age;
            existingPet.Type = dto.Type;
            existingPet.BirthDate = dto.BirthDate;


            await _repository.UpdateAsync(existingPet);
            return true;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var pet = await _repository.GetByIdAsync(id);
            if (pet is null) return false;

            await _repository.DeleteAsync(id);
            return true;
        }
    }
}
