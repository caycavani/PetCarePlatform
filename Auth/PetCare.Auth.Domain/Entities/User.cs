using System;
using System.Collections.Generic;

namespace PetCare.Auth.Domain.Entities
{
    public class User
    {
        public Guid Id { get; private set; }

        public string Email { get; private set; } = string.Empty;

        public string PasswordHash { get; private set; } = string.Empty;

        public string FullName { get; private set; } = string.Empty;

        public string Phone { get; private set; } = string.Empty;

        public string Username { get; private set; } = string.Empty;

        public bool IsActive { get; private set; }

        public DateTime CreatedAt { get; private set; }

        public Guid RoleId { get; private set; }

        public Role Role { get; private set; } = null!;

        public ICollection<RefreshToken> RefreshTokens { get; private set; } = new List<RefreshToken>();

        // 🧬 Constructor protegido para EF Core
        protected User() { }

        /// <summary>
        /// Constructor principal usado al registrar un usuario.
        /// </summary>
        public User(string email, string passwordHash, string username, string fullName, string phone, Guid roleId)
        {
            Id = Guid.NewGuid();
            Email = email.Trim().ToLowerInvariant();
            PasswordHash = passwordHash;
            Username = username.Trim();
            FullName = fullName.Trim();
            Phone = phone.Trim();
            IsActive = true;
            CreatedAt = DateTime.UtcNow;
            RoleId = roleId;
        }

        /// <summary>
        /// Asigna el rol completo al usuario.
        /// </summary>
        public void AssignRole(Role role)
        {
            Role = role;
            RoleId = role.Id;
        }

        /// <summary>
        /// Actualiza datos del perfil del usuario.
        /// </summary>
        public void UpdateProfile(string fullName, string phone)
        {
            FullName = fullName.Trim();
            Phone = phone.Trim();
        }

        /// <summary>
        /// Establece una nueva contraseña.
        /// </summary>
        public void SetPassword(string hashedPassword)
        {
            PasswordHash = hashedPassword;
        }

        /// <summary>
        /// Desactiva el usuario.
        /// </summary>
        public void Deactivate()
        {
            IsActive = false;
        }
    }
}
