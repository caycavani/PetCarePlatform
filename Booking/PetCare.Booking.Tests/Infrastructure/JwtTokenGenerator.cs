using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace PetCare.Booking.Tests.Infrastructure
{
    public static class JwtTokenGenerator
    {
        private const string Secret = "ClaveSeguraParaTestsCon32CharOK!";

        public static string Generate(Guid userId, string role)
        {
            var keyBytes = Encoding.UTF8.GetBytes(Secret);

            if (keyBytes.Length < 32)
            {
                throw new ArgumentOutOfRangeException(nameof(keyBytes),
                    $"La clave usada para HS256 tiene {keyBytes.Length * 8} bits, pero se requieren al menos 256 bits.");
            }

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
                new Claim(ClaimTypes.Role, role) // Estándar para [Authorize(Roles = "Client")]
            };

            var key = new SymmetricSecurityKey(keyBytes);
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: "petcare.test",
                audience: "petcare.test",
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
