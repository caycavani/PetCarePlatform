using PetCare.Auth.Application.DTOs.User;
using PetCare.Auth.Application.Interfaces;
using PetCare.Auth.Domain.Entities;
using PetCare.Auth.Domain.Exceptions;
using PetCare.Auth.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PetCare.Auth.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IRoleRepository _roleRepository;

        public UserService(IUserRepository userRepository, IRoleRepository roleRepository)
        {
            _userRepository = userRepository;
            _roleRepository = roleRepository;
        }

        public async Task<UserDto> GetByIdAsync(Guid id)
        {
            var user = await _userRepository.GetByIdAsync(id)
                       ?? throw new UserNotFoundException(id);

            return new UserDto(user.Id, user.Email, user.FullName, user.Phone, user.IsActive);
        }

        public async Task CreateAsync(CreateUserDto dto, string roleName)
        {
            if (await _userRepository.ExistsAsync(dto.Email))
                throw new InvalidOperationException("El correo ya está en uso.");

            var role = await _roleRepository.GetByNameAsync(roleName);
            if (role is null)
                throw new InvalidOperationException("El rol especificado no existe.");

            var hashedPassword = Hash(dto.Password);

            var user = new User(Guid.NewGuid(), dto.Email, hashedPassword, dto.FullName, dto.Phone,dto.Username);
            user.AssignRole(role);

            await _userRepository.AddAsync(user);
        }

        public async Task UpdateAsync(UpdateUserDto dto)
        {
            var user = await _userRepository.GetByIdAsync(dto.Id)
                       ?? throw new UserNotFoundException(dto.Id);

            user.UpdateProfile(dto.FullName, dto.Phone);

            if (!dto.IsActive)
                user.Deactivate();

            await _userRepository.UpdateAsync(user);
        }

        public async Task<List<UserDto>> GetAllAsync()
        {
            var users = await _userRepository.GetAllAsync();

            return users.Select(u => new UserDto(
                u.Id,
                u.Email,
                u.FullName,
                u.Phone,
                u.IsActive
            )).ToList();
        }

        private string Hash(string rawPassword)
        {
            return Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(rawPassword));
        }
    }
}
