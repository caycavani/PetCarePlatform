using PetCare.Auth.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PetCare.Auth.Application.Interfaces
{
    public interface IRoleService
    {
        Task<List<Role>> GetAllAsync();
        Task<Role?> GetByIdAsync(Guid id);
        Task<Role?> GetByNameAsync(string name);
        Task CreateAsync(string name, string description);
        Task UpdateAsync(Guid id, string name, string description);
        Task DeleteAsync(Guid id);
    }
}
