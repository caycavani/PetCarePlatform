using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PetCare.Auth.Domain.Entities;

namespace PetCare.Auth.Domain.Interfaces
{
    public interface IRoleRepository
    {
        Task<List<Role>> GetAllAsync();
        Task<Role?> GetByIdAsync(Guid id);
        Task<Role?> GetByNameAsync(string name);
        Task<bool> ExistsAsync(string name);
        Task AddAsync(Role role);
        Task UpdateAsync(Role role);
        Task DeleteAsync(Guid id);
    }
}
