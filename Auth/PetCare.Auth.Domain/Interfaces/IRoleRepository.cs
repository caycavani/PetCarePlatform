using PetCare.Auth.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PetCare.Auth.Domain.Interfaces
{
    public interface IRoleRepository
    {
        Task<Role?> GetByIdAsync(Guid id);
        Task<Role?> GetByNameAsync(string name);
        Task<List<Role>> GetAllAsync();
        Task AddAsync(Role role);
        Task UpdateAsync(Role role);
        Task DeleteAsync(Guid id);
        Task<bool> ExistsAsync(string name);
    }
}
