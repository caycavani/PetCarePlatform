namespace PetCare.Notification.Domain.Exceptions
{
    using System;

    /// <summary>
    /// Representa un error de negocio cuando una notificación no cumple con las reglas de validación.
    /// </summary>
    public class InvalidNotificationException : Exception
    {
        /// <summary>
        /// Inicializa una nueva instancia de la excepción con un mensaje personalizado.
        /// </summary>
        /// <param name="message">Mensaje que describe el error.</param>
        public InvalidNotificationException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Inicializa una nueva instancia de la excepción con un mensaje y una excepción interna.
        /// </summary>
        /// <param name="message">Mensaje que describe el error.</param>
        /// <param name="innerException">Excepción que causó el error actual.</param>
        public InvalidNotificationException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
