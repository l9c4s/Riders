using Domain.Contracts.GenericsContrats;
using Domain.Entities;

namespace Domain.Contracts
{
    public interface IUserRepository : IRepository<User>
    {
        Task<User?> GetByEmailAsync(string email);

        Task<User?> GetByUserNameAndEmailAsync(string userName, string email);

        Task<bool> ResetPasswordAsync(Guid id, string newPasswordHash);
    }
}