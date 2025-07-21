using PetCare.Auth.Application.DTOs.Auth;
using PetCare.Auth.Application.DTOs.User;
using PetCare.Auth.Application.Interfaces;
using PetCare.Auth.Domain.Entities;
using PetCare.Auth.Domain.Exceptions;
using PetCare.Auth.Domain.Interfaces;
using System;
using System.Text;
using System.Threading.Tasks;

namespace PetCare.Auth.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly IRefreshTokenRepository _refreshTokenRepository;

        public AuthService(
            IUserRepository userRepository,
            IRoleRepository roleRepository,
            IRefreshTokenRepository refreshTokenRepository)
        {
            _userRepository = userRepository;
            _roleRepository = roleRepository;
            _refreshTokenRepository = refreshTokenRepository;
        }

        public async Task<LoginResultDto> AuthenticateAsync(LoginUserDto dto)
        {
            var user = await _userRepository.GetByEmailAsync(dto.Email)
                       ?? throw new UnauthorizedAccessException("Usuario no encontrado.");

            if (!user.IsActive)
                throw new UnauthorizedAccessException("Usuario desactivado.");

            var hashedInput = Hash(dto.Password);
            if (user.PasswordHash != hashedInput)
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

        public async Task RegisterAsync(CreateUserDto dto, string roleName)
        {
            if (await _userRepository.ExistsAsync(dto.Email))
                throw new InvalidOperationException("El correo ya está en uso.");

            var role = await _roleRepository.GetByNameAsync(roleName.ToLower());
            if (role is null)
                throw new InvalidOperationException("El rol especificado no existe.");

            var hashedPassword = Hash(dto.Password);
            var user = new User(Guid.NewGuid(), dto.Email, hashedPassword, dto.FullName, dto.Phone);
            user.AssignRole(role);

            await _userRepository.AddAsync(user);
        }

        // 🔐 Hashing simple — reemplazar por BCrypt/PBKDF2 en producción
        private string Hash(string input)
        {
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(input));
        }

        // 🪪 Generación básica de AccessToken
        private string GenerateAccessToken(User user)
        {
            return $"access-token-{user.Id}";
        }

        // 🔁 Generación básica de RefreshToken
        private string GenerateRefreshToken()
        {
            return Guid.NewGuid().ToString("N");
        }
    }
}
