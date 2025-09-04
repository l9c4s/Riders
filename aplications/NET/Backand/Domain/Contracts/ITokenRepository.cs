using Domain.Contracts.GenericsContrats;
using Domain.Entities;

namespace Domain.Contracts;

public interface ITokenRepository : IRepository<RefreshToken>
{

    Task<RefreshToken> GetByTokenAsync(string token);


}
