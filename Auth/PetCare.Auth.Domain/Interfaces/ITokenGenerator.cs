using PetCare.Auth.Domain.Entities;
using PetCare.Auth.Domain.Results;

namespace PetCare.Auth.Domain.Interfaces
{
    public interface ITokenGenerator
    {
        TokenResult GenerateAccessToken(User user);
        TokenResult GenerateRefreshToken();
    }
}
