using Domain.Contracts;
using Domain.Entities;
using Infraestructure.Data;
using Infraestructure.Repositories.GenericsRepositories;
using Microsoft.EntityFrameworkCore;

namespace Infraestructure.Repositories;

public class LocationShareRepository : Repository<LocationShare>, ILocationShareRepository
{
    public LocationShareRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<LocationShare?> FindShareAsync(Guid sharerId, Guid observerId)
    {
        return await _context.LocationShares
        .FirstOrDefaultAsync(ls => ls.SharerId == sharerId && ls.ObserverId == observerId);
    }

    public async Task<IEnumerable<LocationShare>> GetActiveSharesBySharerAsync(Guid sharerId)
    {
        return await _context.LocationShares
        .Where(s => s.SharerId == sharerId && s.IsActive)
        .ToListAsync();

    }

    public async Task<IEnumerable<LocationShare>> GetActiveSharesForObserverAsync(Guid observerId)
    {
        return await _dbSet
            .Where(ls => ls.ObserverId == observerId && ls.IsActive)
            .ToListAsync();
    }
}
