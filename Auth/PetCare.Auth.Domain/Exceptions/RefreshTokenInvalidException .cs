using System;

namespace PetCare.Auth.Domain.Exceptions
{
    public class RefreshTokenInvalidException : Exception
    {
        public RefreshTokenInvalidException(Guid userId)
            : base($"El token de refresco no es válido para el usuario con ID '{userId}'.") { }

        public RefreshTokenInvalidException(string message)
            : base(message) { }
    }
}
