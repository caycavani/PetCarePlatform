using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using PetCare.Auth.Domain.Entities;
using PetCare.Auth.Domain.Interfaces;

namespace PetCare.Auth.Infrastructure.Seeding
{
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

            // Contraseña temporal
            var tempPassword = "123456";

            // Usuario administrador
            var admin = new User(
                email: "admin@pets.com",
                passwordHash: "", // Se establecerá después
                username: "admin",
                fullName: "Administrador Principal",
                phone: "+57 000 000 0000",
                roleId: role.Id
            );

            // Asignación de rol con navegación
            admin.AssignRole(role);

            // Asignación segura de contraseña
            var hashedPassword = _passwordHasher.HashPassword(admin, tempPassword);
            admin.SetPassword(hashedPassword);

            await _userRepository.AddAsync(admin);
        }
    }
}
