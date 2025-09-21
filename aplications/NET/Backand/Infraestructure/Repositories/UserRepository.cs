using Domain.Contracts;
using Domain.Entities;
using Infraestructure.Data;
using Infraestructure.Repositories.GenericsRepositories;
using Microsoft.EntityFrameworkCore;

namespace Infraestructure.Repositories
{
    public class UserRepository : Repository<User>, IUserRepository
    {
        public UserRepository(AppDbContext context) : base(context) { 
        
        
        }

        public async Task<User?> GetByEmailAsync(string email)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
        }

        public Task<User?> GetByUserNameAndEmailAsync(string userName, string email)
        {
            return _context.Users.FirstOrDefaultAsync(u => u.Email == email && u.UserName == userName);
        }

        public async Task<bool> ResetPasswordAsync(Guid id, string newPasswordHash)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
            
            user.PasswordHash = newPasswordHash;
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
