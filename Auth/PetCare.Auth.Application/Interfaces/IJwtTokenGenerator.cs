namespace PetCare.Auth.Application.Interfaces
{
    public interface IJwtTokenGenerator
    {
        Task<string> GenerateAsync(string email);
        string GenerateToken(Guid userId, string role);
    }
}
