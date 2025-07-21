using System;

namespace PetCare.Auth.Domain.Entities
{
    public class Role
    {
        public Guid Id { get; private set; }

        public string Name { get; private set; } = string.Empty;

        public string NormalizedName { get; private set; } = string.Empty;

        protected Role() { } // requerido por EF Core

        /// <summary>
        /// Constructor simple, ideal para semilla o registro manual.
        /// </summary>
        public Role(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("El nombre del rol no puede estar vacío.", nameof(name));

            Id = Guid.NewGuid();
            Name = name.Trim().ToLower();
            NormalizedName = name.Trim().ToUpper();
        }

        /// <summary>
        /// Alternativa si quieres permitir cambiar el nombre (opcional).
        /// </summary>
        public void Rename(string newName)
        {
            if (string.IsNullOrWhiteSpace(newName))
                throw new ArgumentException("El nuevo nombre no puede estar vacío.", nameof(newName));

            Name = newName.Trim().ToLower();
            NormalizedName = newName.Trim().ToUpper();
        }
    }
}
