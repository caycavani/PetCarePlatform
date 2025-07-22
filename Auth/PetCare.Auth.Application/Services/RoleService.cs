using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PetCare.Auth.Domain.Entities;
using PetCare.Auth.Domain.Interfaces;
using PetCare.Auth.Application.Interfaces;
using PetCare.Auth.Application.DTOs.Roles;

namespace PetCare.Auth.Application.Services
{
    public class RoleService : IRoleService
    {
        private readonly IRoleRepository _repository;

        public RoleService(IRoleRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<Role>> GetAllAsync()
        {
            return await _repository.GetAllAsync();
        }

        public async Task<Role?> GetByIdAsync(Guid id)
        {
            return await _repository.GetByIdAsync(id);
        }

        public async Task<Guid> CreateAsync(CreateRoleDto dto)
        {
            if (dto == null || string.IsNullOrWhiteSpace(dto.Name))
                throw new ArgumentException("El nombre del rol no puede estar vacío.");

            var exists = await _repository.ExistsAsync(dto.Name);
            if (exists)
                throw new InvalidOperationException("Ya existe un rol con ese nombre.");

            var role = new Role(dto.Name);
            await _repository.AddAsync(role);
            return role.Id;
        }

        public async Task UpdateAsync(Guid id, CreateRoleDto dto)
        {
            if (dto == null || string.IsNullOrWhiteSpace(dto.Name))
                throw new ArgumentException("El nombre del rol no puede estar vacío.");

            var role = await _repository.GetByIdAsync(id);
            if (role is null)
                throw new KeyNotFoundException("No se encontró el rol para actualizar.");

            role.Rename(dto.Name);
            await _repository.UpdateAsync(role);
        }

        public async Task DeleteAsync(Guid id)
        {
            var role = await _repository.GetByIdAsync(id);
            if (role is null)
                throw new KeyNotFoundException("No se encontró el rol para eliminar.");

            await _repository.DeleteAsync(id);
        }
    }
}
