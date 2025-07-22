using System;
using BCrypt.Net;
using PetCare.Auth.Domain.Entities;

namespace PetCare.Auth.Domain.Entities
{
    public class User
    {
        public Guid Id { get; private set; }

        public string Email { get; private set; } = string.Empty;
        public string PasswordHash { get; private set; } = string.Empty;
        public string Username { get; private set; } = string.Empty;
        public string FullName { get; private set; } = string.Empty;
        public string Phone { get; private set; } = string.Empty;

        public bool IsActive { get; private set; } = true;
        public DateTime CreatedAt { get; private set; }

        // 🔗 Relación con Rol
        public Guid RoleId { get; private set; }
        public Role Role { get; private set; } = default!;

        // 🔐 Constructor protegido para EF Core
        protected User() { }

        public User(Guid id, string email, string passwordHash, string fullName, string phone, string username)
        {
            Id = id;
            Email = email;
            PasswordHash = passwordHash;
            FullName = fullName;
            Phone = phone;
            Username = username;
            IsActive = true;
            CreatedAt = DateTime.UtcNow;
        }

        // 🛠️ Asignar rol
        public void AssignRole(Role role)
        {
            if (role is null)
                throw new ArgumentNullException(nameof(role));

            Role = role;
            RoleId = role.Id;
        }

        // 🔐 Establecer contraseña con hash seguro
        public void SetPassword(string plainPassword)
        {
            if (string.IsNullOrWhiteSpace(plainPassword))
                throw new ArgumentException("La contraseña no puede estar vacía.");

            PasswordHash = BCrypt.Net.BCrypt.HashPassword(plainPassword);
        }

        // 🔐 Verificar contraseña
        public bool VerifyPassword(string input)
        {
            return BCrypt.Net.BCrypt.Verify(input, PasswordHash);
        }

        // 📞 Actualizar perfil
        public void UpdateProfile(string fullName, string phone)
        {
            FullName = fullName;
            Phone = phone;
        }

        // 🚫 Desactivar cuenta
        public void Deactivate()
        {
            IsActive = false;
        }
    }
}
