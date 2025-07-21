using System;

namespace PetCare.Auth.Domain.Exceptions
{
    public class RoleNotFoundException : Exception
    {
        public RoleNotFoundException(Guid roleId)
            : base($"No se encontró el rol con ID '{roleId}'.") { }

        public RoleNotFoundException(string roleName)
            : base($"No se encontró el rol llamado '{roleName}'.") { }
    }
}
