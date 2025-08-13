using PetCare.Auth.Application.Interfaces;
using System;
using System.Threading.Tasks;

namespace PetCare.Auth.Infrastructure.Security
{
    public class JwtTokenGeneratorMock : IJwtTokenGenerator
    {
        public Task<string> GenerateAsync(string email)
        {
            // Simula un token usando el email como referencia
            return Task.FromResult($"mock-token-for-{email}");
        }

        public string GenerateToken(Guid userId, string role)
        {
            // Simula un token usando el GUID y el rol
            return $"mock-token-for-{userId}-role-{role}";
        }
    }
}
