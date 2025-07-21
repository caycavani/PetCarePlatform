
using System;
namespace PetCare.Auth.Domain.Entities
{
    using System;

    /// <summary>
    /// Represents an active session for a user.
    /// </summary>
    public class UserSession
    {
        /// <summary>
        /// Gets or sets the unique identifier of the session.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the user associated with the session.
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// Gets or sets the IP address of the session origin.
        /// </summary>
        public string IpAddress { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the user agent string.
        /// </summary>
        public string UserAgent { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the session start time.
        /// </summary>
        public DateTime StartedAt { get; set; }

        /// <summary>
        /// Gets or sets the session expiration time.
        /// </summary>
        public DateTime ExpiresAt { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether gets or sets whether the session is currently active.
        /// </summary>
        public bool IsActive { get; set; }
    }
}
