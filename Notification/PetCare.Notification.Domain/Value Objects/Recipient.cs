namespace PetCare.Notification.Domain.Value_Objects
{
    using System;

    /// <summary>
    /// Value Object que representa al destinatario de una notificación.
    /// </summary>
    public class Recipient
    {
        public string Name { get; }
        public string Email { get; }
        public string PhoneNumber { get; }
        public string? DeviceToken { get; }

        public Recipient(string name, string email, string phoneNumber, string? deviceToken = null)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("El nombre del destinatario no puede estar vacío.", nameof(name));

            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException("El correo electrónico del destinatario no puede estar vacío.", nameof(email));

            if (string.IsNullOrWhiteSpace(phoneNumber))
                throw new ArgumentException("El número de teléfono del destinatario no puede estar vacío.", nameof(phoneNumber));

            Name = name;
            Email = email;
            PhoneNumber = phoneNumber;
            DeviceToken = deviceToken; // Puede ser null si el canal no es push
        }
    }
}
