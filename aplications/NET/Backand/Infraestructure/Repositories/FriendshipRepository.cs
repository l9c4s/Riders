using Domain.Contracts;
using Domain.Entities;
using Infraestructure.Data;
using Infraestructure.Repositories.GenericsRepositories;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;


namespace Infraestructure.Repositories
{
    public class FriendshipRepository : Repository<Friendship>, IFriendshipRepository
    {
        public FriendshipRepository(AppDbContext context) : base(context) { }

        public async Task<Friendship?> FirstOrDefaultAsync(Expression<Func<Friendship, bool>> predicate)
        {
            return await _context.Set<Friendship>().FirstOrDefaultAsync(predicate);
        }

        public async Task<IEnumerable<Friendship>> FindAsync(Expression<Func<Friendship, bool>> predicate)
        {
            return await _context.Set<Friendship>()
                                 .Include(f => f.Addressee)
                                 .Where(predicate).ToListAsync();
        }

        public async Task<IEnumerable<Friendship>> GetFriendRequests(Expression<Func<Friendship, bool>> predicate)
        {
            return await _context.Set<Friendship>()
                                 .Include(f => f.Requester)
                                 .Where(predicate).ToListAsync();
        }
    }
}
