using Domain.Dto.Token;

namespace Aplication.Contracts;

public interface ITokenUseCases
{
   Task<TokenResponse> GenerateToken(string userId, string email);
   Task<string> RefreshToken(string refreshToken,string TokenJwt);
}
