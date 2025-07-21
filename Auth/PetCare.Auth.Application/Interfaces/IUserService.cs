using PetCare.Auth.Application.DTOs.User;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PetCare.Auth.Application.Interfaces
{
    public interface IUserService
    {
        Task<UserDto> GetByIdAsync(Guid id);
        Task CreateAsync(CreateUserDto dto, string roleName);
        Task UpdateAsync(UpdateUserDto dto);
        Task<List<UserDto>> GetAllAsync();

    }
}
