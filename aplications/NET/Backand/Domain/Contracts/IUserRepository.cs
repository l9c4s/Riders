using Domain.Contracts.GenericsContrats;
using Domain.Entities;

namespace Domain.Contracts
{
    public interface IUserRepository : IRepository<User>
    {
        Task<User?> GetByEmailAsync(string email);

        Task<User?> GetByUserNameAndEmail(string userName, string email);

        Task<User?> GetByUserNameAndEmailandNickNameAsync(string userName, string email, string nickName);

        Task<bool> ResetPasswordAsync(Guid id, string newPasswordHash);
    }
}