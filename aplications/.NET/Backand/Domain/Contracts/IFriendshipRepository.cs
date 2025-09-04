using Domain.Contracts.GenericsContrats;
using Domain.Entities;

namespace Domain.Contracts
{
    public interface IFriendshipRepository : IRepository<Friendship>
    {
        Task<Friendship?> FirstOrDefaultAsync(System.Linq.Expressions.Expression<System.Func<Friendship, bool>> predicate);
        Task<System.Collections.Generic.IEnumerable<Friendship>> FindAsync(System.Linq.Expressions.Expression<System.Func<Friendship, bool>> predicate);

        Task<System.Collections.Generic.IEnumerable<Friendship>> GetFriendRequests(System.Linq.Expressions.Expression<System.Func<Friendship, bool>> predicate);
    }
}
