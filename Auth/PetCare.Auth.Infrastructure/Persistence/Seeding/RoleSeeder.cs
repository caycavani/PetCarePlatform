using PetCare.Auth.Domain.Entities;
using PetCare.Auth.Domain.Interfaces;
using System;
using System.Threading.Tasks;

namespace PetCare.Auth.Infrastructure.Persistence.Seeding
{
    public class RoleSeeder
    {
        private readonly IRoleRepository _roleRepository;

        public RoleSeeder(IRoleRepository roleRepository)
        {
            _roleRepository = roleRepository;
        }

        public async Task SeedAsync()
        {
            var baseRoles = new[] { "admin", "user" };

            foreach (var roleName in baseRoles)
            {
                var existing = await _roleRepository.GetByNameAsync(roleName);
                if (existing is null)
                {
                    var role = new Role(roleName);

                    await _roleRepository.AddAsync(role);
                }
            }
        }
    }
}
