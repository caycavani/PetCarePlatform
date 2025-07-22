using BCrypt.Net;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using PetCare.Auth.Application.DTOs.Auth;
using PetCare.Auth.Application.DTOs.User;
using PetCare.Auth.Application.Interfaces;
using PetCare.Auth.Domain.Entities;
using PetCare.Auth.Domain.Exceptions;
using PetCare.Auth.Domain.Interfaces;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace PetCare.Auth.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        private readonly IConfiguration _config;

        public AuthService(
            IUserRepository userRepository,
            IRoleRepository roleRepository,
            IRefreshTokenRepository refreshTokenRepository,
            IConfiguration config)
        {
            _userRepository = userRepository;
            _roleRepository = roleRepository;
            _refreshTokenRepository = refreshTokenRepository;
            _config = config;
        }

        public async Task RegisterAsync(CreateUserDto dto, string roleName)
        {
            if (dto is null)
                throw new ArgumentNullException(nameof(dto));

            var email = dto.Email?.Trim().ToLower();
            if (await _userRepository.ExistsAsync(email))
                throw new InvalidOperationException("El correo ya está en uso.");

            var normalizedRole = roleName?.Trim().ToUpper();
            var role = await _roleRepository.GetByNameAsync(normalizedRole);
            if (role is null)
                throw new InvalidOperationException("El rol especificado no existe.");

            var hashedPassword = Hash(dto.Password);

            var user = new User(
                Guid.NewGuid(),
                email!,
                hashedPassword,
                dto.FullName,
                dto.Phone,
                dto.Username
            );

            user.AssignRole(role);
            await _userRepository.AddAsync(user);
        }

        public async Task<LoginResultDto> AuthenticateAsync(LoginUserDto dto)
        {
            if (dto is null)
                throw new ArgumentNullException(nameof(dto));

            var identifier = dto.Identifier?.Trim().ToLower();
            if (string.IsNullOrWhiteSpace(identifier))
                throw new UnauthorizedAccessException("Debe proporcionar un correo o usuario.");

            User? user = IsEmail(identifier)
                ? await _userRepository.GetByEmailAsync(identifier)
                : await _userRepository.GetByUsernameAsync(identifier);

            if (user is null)
                throw new UnauthorizedAccessException("Usuario no encontrado.");

            if (!user.IsActive)
                throw new UnauthorizedAccessException("Usuario desactivado.");

            if (!VerifyPassword(dto.Password, user.PasswordHash))
                throw new UnauthorizedAccessException("Credenciales inválidas.");

            var accessToken = GenerateAccessToken(user);
            var refreshToken = GenerateRefreshToken();

            var tokenEntity = new RefreshToken(Guid.NewGuid(), user.Id, refreshToken);
            await _refreshTokenRepository.AddAsync(tokenEntity);

            return new LoginResultDto(accessToken, refreshToken);
        }

        public async Task<TokenResultDto> RefreshTokenAsync(Guid userId, string refreshToken)
        {
            var token = await _refreshTokenRepository.GetByUserAndTokenAsync(userId, refreshToken);
            if (token is null || token.IsExpired)
                throw new UnauthorizedAccessException("Refresh token inválido o expirado.");

            var user = await _userRepository.GetByIdAsync(userId)
                       ?? throw new UserNotFoundException(userId);

            var newAccessToken = GenerateAccessToken(user);
            var newRefreshToken = GenerateRefreshToken();

            token.Rotate(newRefreshToken);
            await _refreshTokenRepository.UpdateAsync(token);

            return new TokenResultDto(newAccessToken, newRefreshToken);
        }

        private string Hash(string input)
        {
            return BCrypt.Net.BCrypt.HashPassword(input);
        }

        private bool VerifyPassword(string input, string storedHash)
        {
            return BCrypt.Net.BCrypt.Verify(input, storedHash);
        }

        private string GenerateAccessToken(User user)
        {
            var claims = new[]
            {
        new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
        new Claim(ClaimTypes.Name, user.Username),
        new Claim(ClaimTypes.Email, user.Email),
        new Claim(ClaimTypes.Role, user.Role?.Name ?? "CLIENTE") // Ajusta si el rol está asignado
    };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Secret"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public async Task UpdateProfileAsync(Guid userId, UpdateProfileDto dto)
        {
            var user = await _userRepository.GetByIdAsync(userId)
                ?? throw new UserNotFoundException(userId);

            user.UpdateProfile(dto.FullName, dto.Phone);
            await _userRepository.UpdateAsync(user);
        }


        private string GenerateRefreshToken()
        {
            return Guid.NewGuid().ToString("N");
        }

        private bool IsEmail(string value)
        {
            return Regex.IsMatch(value, @"^[^@\s]+@[^@\s]+\.[^@\s]+$");
        }
    }
}
