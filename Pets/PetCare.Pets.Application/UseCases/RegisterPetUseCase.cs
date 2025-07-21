namespace PetCare.Pets.Application.UseCases
{
    using PetCare.Pets.Application.DTOs;
    using PetCare.Pets.Domain.Entities;
    using PetCare.Pets.Domain.Interfaces;

    public class RegisterPetUseCase
    {
        private readonly IPetRepository _repository;

        public RegisterPetUseCase(IPetRepository repository)
        {
            _repository = repository;
        }

        public async Task<Guid> ExecuteAsync(CreatePetDto dto)
        {
            var pet = new Pet(
                ownerId: dto.OwnerId,
                name: dto.Name,
                type: dto.Type,
                birthDate: dto.BirthDate
            );

            await _repository.AddAsync(pet);

            return pet.Id;
        }
    }
}
