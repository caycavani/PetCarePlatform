using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Tokens;
using PetCare.Auth.Application.Interfaces;

namespace PetCare.Auth.Application.Services
{
    public class JwtTokenGenerator : IJwtTokenGenerator
    {
        // 🔐 Ajustes embebidos directamente (solo para pruebas locales)
        private const string Secret = "TuClaveSuperSecreta123!CambioEstoEnProduccion";
        private const string Issuer = "PetCare.Auth";
        private const string Audience = "PetCare.Pets";
        private const string Environment = "Development";

        /// <summary>
        /// Genera un token JWT basado en el email. Convierte email en GUID simulado para test.
        /// </summary>
        public Task<string> GenerateAsync(string email)
        {
            var simulatedUserId = Guid.NewGuid();
            var token = GenerateToken(simulatedUserId, "User");
            return Task.FromResult(token);
        }

        /// <summary>
        /// Genera un token JWT basado en GUID y rol explícito, sin incluir 'kid' en el encabezado.
        /// </summary>
        public string GenerateToken(Guid userId, string role)
        {
            if (string.IsNullOrWhiteSpace(Secret))
                throw new InvalidOperationException("JWT Secret is missing.");

            var keyBytes = Encoding.UTF8.GetBytes(Secret);
            var securityKey = new SymmetricSecurityKey(keyBytes);

            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, userId.ToString()),
                new Claim(ClaimTypes.Role, role),
                new Claim("env", Environment),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddHours(1),
                Issuer = Issuer,
                Audience = Audience,
                SigningCredentials = credentials
            };

            var handler = new JwtSecurityTokenHandler();
            var token = handler.CreateToken(tokenDescriptor);
            var jwt = token as JwtSecurityToken;

            // 🧪 Diagnóstico: imprimir encabezado del token
            Console.WriteLine("🔍 Encabezado del token:");
            foreach (var kvp in jwt.Header)
            {
                Console.WriteLine($"🔸 {kvp.Key}: {kvp.Value}");
            }

            return handler.WriteToken(jwt);
        }
    }
}
