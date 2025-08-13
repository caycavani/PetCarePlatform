using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using PetCare.Auth.Application.Interfaces;

namespace PetCare.Tests.Mocks
{
    public class FakeJwtTokenGenerator : IJwtTokenGenerator
    {
        private readonly IConfiguration _configuration;

        public FakeJwtTokenGenerator(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<string> GenerateAsync(string email)
        {
            // Puedes usar un Guid fijo o simulado para pruebas
            var userId = Guid.NewGuid();
            return await Task.FromResult(GenerateToken(userId, "CLIENTE"));
        }

        public string GenerateToken(Guid userId, string role)
        {
            var key = Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]);
            var issuer = _configuration["Jwt:Issuer"];

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, userId.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, $"fake_{userId}@example.com"),
                new Claim(ClaimTypes.Role, role),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var creds = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: issuer,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(60),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
