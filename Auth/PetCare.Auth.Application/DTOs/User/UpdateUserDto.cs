using System;

namespace PetCare.Auth.Application.DTOs.User
{
    public class UpdateUserDto
    {
        public Guid Id { get; set; }
        public string FullName { get; set; }
        public string Phone { get; set; }
        public bool IsActive { get; set; }

        public UpdateUserDto() { }

        public UpdateUserDto(Guid id, string fullName, string phone, bool isActive)
        {
            Id = id;
            FullName = fullName;
            Phone = phone;
            IsActive = isActive;
        }
    }
}
