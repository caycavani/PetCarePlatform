using PetCare.Auth.Application.Interfaces;
using PetCare.Auth.Domain.Entities;
using PetCare.Auth.Domain.Exceptions;
using PetCare.Auth.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PetCare.Auth.Application.Services
{
    public class RoleService : IRoleService
    {
        private readonly IRoleRepository _roleRepository;

        public RoleService(IRoleRepository roleRepository)
        {
            _roleRepository = roleRepository;
        }

        public async Task<List<Role>> GetAllAsync()
        {
            return await _roleRepository.GetAllAsync();
        }

        public async Task<Role?> GetByIdAsync(Guid id)
        {
            return await _roleRepository.GetByIdAsync(id);
        }

        public async Task<Role?> GetByNameAsync(string name)
        {
            return await _roleRepository.GetByNameAsync(name.ToLower());
        }

        public async Task CreateAsync(string name, string description)
        {
            if (await _roleRepository.GetByNameAsync(name.ToLower()) is not null)
                throw new InvalidOperationException("Ya existe un rol con ese nombre.");

            var role = new Role(name);
            await _roleRepository.AddAsync(role);
        }

        public async Task UpdateAsync(Guid id, string name, string description)
        {
            var role = await _roleRepository.GetByIdAsync(id)
                        ?? throw new RoleNotFoundException(id);

            role.Rename(name); // Este método debería existir en tu entidad Role

            await _roleRepository.UpdateAsync(role);
        }

        public async Task DeleteAsync(Guid id)
        {
            var role = await _roleRepository.GetByIdAsync(id)
                        ?? throw new RoleNotFoundException(id);

            await _roleRepository.DeleteAsync(role.Id);
        }
    }
}
