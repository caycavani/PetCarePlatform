using System;

namespace PetCare.Auth.Domain.Entities
{
    public class User
    {
        public Guid Id { get; private set; }

        public string Email { get; private set; } = string.Empty;
        public string PasswordHash { get; private set; } = string.Empty;
        public string FullName { get; private set; } = string.Empty;
        public string Phone { get; private set; } = string.Empty;

        public bool IsActive { get; private set; } = true;

        // 🏷️ Relación con Rol
        public Guid RoleId { get; private set; }
        public Role Role { get; private set; } = default!;

        public DateTime CreatedAt { get; private set; }

        // 📦 Constructor protegido para EF Core
        protected User() { }

        public User(Guid id, string email, string passwordHash, string fullName, string phone)
        {
            Id = id;
            Email = email;
            PasswordHash = passwordHash;
            FullName = fullName;
            Phone = phone;
            IsActive = true;
            CreatedAt = DateTime.UtcNow;
        }

        // 🛠️ Métodos de negocio
        public void AssignRole(Role role)
        {
            if (role is null)
                throw new ArgumentNullException(nameof(role));

            Role = role;
            RoleId = role.Id;
        }

        public void SetPasswordHash(string hash)
        {
            PasswordHash = hash;
        }


        public void UpdateProfile(string fullName, string phone)
        {
            FullName = fullName;
            Phone = phone;
        }

        public void Deactivate()
        {
            IsActive = false;
        }
    }
}
