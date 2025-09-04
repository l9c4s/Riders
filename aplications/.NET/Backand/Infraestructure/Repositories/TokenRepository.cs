using System;
using Domain.Contracts;
using Domain.Entities;
using Infraestructure.Data;
using Infraestructure.Repositories.GenericsRepositories;
using Microsoft.EntityFrameworkCore;

namespace Infraestructure.Repositories;

public class TokenRepository : Repository<RefreshToken>, ITokenRepository
{
    public TokenRepository(AppDbContext context) : base(context) { }

    public async Task<RefreshToken?> GetByTokenAsync(string token)
    {
        var refreshToken = await _context.RefreshTokens
            .FirstOrDefaultAsync(rt => rt.Token == token);

        if (refreshToken == null)
        {
            throw new ArgumentException("Refresh token not found");
        }
        return refreshToken;
    }
}
