using System;
using System.Collections.Generic;

namespace PetCare.Auth.Domain.Entities
{
    public class Role
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string NormalizedName { get; set; } = string.Empty;

        public ICollection<User> Users { get; set; } = new List<User>();

        protected Role() { }

        public Role(string name)
        {
            SetName(name);
            Id = Guid.NewGuid();
        }

        public Role(string name, string normalizedName)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("El nombre del rol no puede estar vacío.", nameof(name));

            if (string.IsNullOrWhiteSpace(normalizedName))
                throw new ArgumentException("El nombre normalizado no puede estar vacío.", nameof(normalizedName));

            Id = Guid.NewGuid();
            Name = name.Trim().ToLowerInvariant();
            NormalizedName = normalizedName.Trim().ToUpperInvariant();
        }

        public void Rename(string newName)
        {
            SetName(newName);
        }

        private void SetName(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("El nombre no puede estar vacío.", nameof(value));

            Name = value.Trim().ToLowerInvariant();
            NormalizedName = value.Trim().ToUpperInvariant();
        }
    }
}
