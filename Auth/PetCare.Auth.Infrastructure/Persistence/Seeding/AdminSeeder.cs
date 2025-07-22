namespace PetCare.Auth.Infrastructure.Seeding
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Identity;
    using PetCare.Auth.Domain.Entities;
    using PetCare.Auth.Domain.Interfaces;

    public class AdminSeeder
    {
        private readonly IUserRepository _userRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly IPasswordHasher<User> _passwordHasher;

        public AdminSeeder(
            IUserRepository userRepository,
            IRoleRepository roleRepository,
            IPasswordHasher<User> passwordHasher)
        {
            _userRepository = userRepository;
            _roleRepository = roleRepository;
            _passwordHasher = passwordHasher;
        }

        public async Task SeedAsync()
        {
            var exists = await _userRepository.ExistsAsync("admin@pets.com");
            if (exists) return;

            var role = await _roleRepository.GetByNameAsync("admin");
            if (role is null)
                throw new InvalidOperationException("El rol 'admin' no existe.");

            var admin = new User(
                Guid.NewGuid(),
                "admin@pets.com",
                "", // password temporal
                "Administrador Principal",
                "+57 000 000 0000",
                "admin@pets.com"
            );

            admin.AssignRole(role);

            // Asignación segura de contraseña
            var hash = _passwordHasher.HashPassword(admin, "123456");
           // admin.SetPasswordHash(hash); // 👈 Este método debe existir como público o interno en User

            await _userRepository.AddAsync(admin);
        }
    }
}
