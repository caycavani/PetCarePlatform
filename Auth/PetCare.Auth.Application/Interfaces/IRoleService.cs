using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PetCare.Auth.Domain.Entities;
using PetCare.Auth.Application.DTOs.Roles;

namespace PetCare.Auth.Application.Interfaces
{
    public interface IRoleService
    {
        Task<List<Role>> GetAllAsync();
        Task<Role?> GetByIdAsync(Guid id);
        Task<Guid> CreateAsync(CreateRoleDto dto);
        Task UpdateAsync(Guid id, CreateRoleDto dto);
        Task DeleteAsync(Guid id);
    }
}
