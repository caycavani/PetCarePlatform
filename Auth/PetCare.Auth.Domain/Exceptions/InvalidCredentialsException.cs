using System;

namespace PetCare.Auth.Domain.Exceptions
{
    public class InvalidCredentialsException : Exception
    {
        public InvalidCredentialsException()
            : base("Credenciales inv�lidas.") { }

        public InvalidCredentialsException(string message)
            : base(message) { }
    }
}
