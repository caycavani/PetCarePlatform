using System;

namespace PetCare.Auth.Domain.Exceptions
{
    public class UserNotFoundException : Exception
    {
        public UserNotFoundException(Guid userId)
            : base($"No se encontró un usuario con ID '{userId}'.") { }

        public UserNotFoundException(string email)
            : base($"No se encontró un usuario con correo '{email}'.") { }

        public UserNotFoundException(string message, Exception inner)
            : base(message, inner) { }
    }
}
