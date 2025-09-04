using Domain.Contracts.GenericsContrats;
using Domain.Entities;

namespace Domain.Contracts;

public interface ILocationShareRepository : IRepository<LocationShare>
{
    Task<IEnumerable<LocationShare>> GetActiveSharesForObserverAsync(Guid observerId);
    Task<LocationShare?> FindShareAsync(Guid sharerId, Guid observerId);

    Task<IEnumerable<LocationShare>> GetActiveSharesBySharerAsync(Guid sharerId);
}
