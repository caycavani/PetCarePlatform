namespace PetCare.Auth.Infrastructure.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using System.Xml.Linq;
    using Microsoft.EntityFrameworkCore;
    using PetCare.Auth.Application.Interfaces;
    using PetCare.Auth.Domain.Entities;
    using PetCare.Auth.Domain.Interfaces;
    using PetCare.Auth.Infrastructure.Persistence;

    public class RoleRepository : IRoleRepository
    {
        private readonly AuthDbContext _context;

        public RoleRepository(AuthDbContext context)
        {
            _context = context;
        }

        public async Task<List<Role>> GetAllAsync()
        {
            return await _context.Roles.ToListAsync();
        }

        public async Task<Role?> GetByIdAsync(Guid id)
        {
            return await _context.Roles
                .FirstOrDefaultAsync(r => r.Id == id);
        }

        public async Task<Role?> GetByNameAsync(string name)
        {
            var normalized = name?.Trim().ToUpper();
            return await _context.Roles
                .FirstOrDefaultAsync(r => r.NormalizedName == normalized);
        }

        public async Task<bool> ExistsAsync(string name)
        {
            var normalized = name?.Trim().ToUpper();
            return await _context.Roles.AnyAsync(r => r.NormalizedName == normalized);
        }

        public async Task AddAsync(Role role)
        {
            await _context.Roles.AddAsync(role);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Role role)
        {
            var existingRole = await GetByIdAsync(role.Id);
            if (existingRole is not null)
            {
                existingRole.Rename(role.Name);
                _context.Roles.Update(existingRole);
                await _context.SaveChangesAsync();
            }
        }

        public async Task DeleteAsync(Guid id)
        {
            var role = await GetByIdAsync(id);
            if (role is not null)
            {
                _context.Roles.Remove(role);
                await _context.SaveChangesAsync();
            }
        }
    }
}
