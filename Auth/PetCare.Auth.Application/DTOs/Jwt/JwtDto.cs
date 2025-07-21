using System;
using System.Collections.Generic;

namespace PetCare.Auth.Application.DTOs.Jwt
{
    public class JwtDto
    {
        public string Token { get; set; }
        public string RefreshToken { get; set; }
        public string Username { get; set; }
        public List<string> Roles { get; set; }
        public DateTime Expiration { get; set; }
        public string TokenType { get; set; }

        public JwtDto(
            string token,
            string refreshToken,
            string username,
            List<string> roles,
            DateTime expiration,
            string tokenType)
        {
            Token = token;
            RefreshToken = refreshToken;
            Username = username;
            Roles = roles;
            Expiration = expiration;
            TokenType = tokenType;
        }
    }
}
