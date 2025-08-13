using System;

namespace PetCare.Auth.Domain.Entities
{
    public class RefreshToken
    {
        public Guid Id { get; private set; }

        public Guid UserId { get; private set; }

        public string Token { get; private set; } = string.Empty;

        public DateTime ExpiresAt { get; private set; }

        // 🔁 Navegación hacia User
        public User User { get; private set; } = null!;

        // 🧬 Constructor protegido para EF Core
        protected RefreshToken() { }

        /// <summary>
        /// Constructor principal. Crea un token con expiración automática.
        /// </summary>
        public RefreshToken(Guid id, Guid userId, string token, int daysValid = 7)
        {
            if (string.IsNullOrWhiteSpace(token))
                throw new ArgumentException("El token no puede estar vacío.", nameof(token));

            Id = id;
            UserId = userId;
            Token = token.Trim();
            ExpiresAt = DateTime.UtcNow.AddDays(daysValid);
        }

        /// <summary>
        /// Verifica si el token ya expiró.
        /// </summary>
        public bool IsExpired => DateTime.UtcNow >= ExpiresAt;

        /// <summary>
        /// Rota el token por uno nuevo y reinicia la fecha de expiración.
        /// </summary>
        public void Rotate(string newToken, int daysValid = 7)
        {
            if (string.IsNullOrWhiteSpace(newToken))
                throw new ArgumentException("El nuevo token no puede estar vacío.", nameof(newToken));

            Token = newToken.Trim();
            ExpiresAt = DateTime.UtcNow.AddDays(daysValid);
        }
    }
}
