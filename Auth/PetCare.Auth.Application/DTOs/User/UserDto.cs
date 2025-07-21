using System;

namespace PetCare.Auth.Application.DTOs.User
{
    public class UserDto
    {
        public Guid Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public bool IsActive { get; set; }

        public UserDto() { }

        public UserDto(Guid id, string email, string fullName, string phone, bool isActive)
        {
            Id = id;
            Email = email;
            FullName = fullName;
            Phone = phone;
            IsActive = isActive;
        }
    }
}
