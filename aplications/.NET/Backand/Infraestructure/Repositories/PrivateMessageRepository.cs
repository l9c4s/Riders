using Domain.Contracts;
using Domain.Entities;
using Infraestructure.Data;
using Infraestructure.Repositories.GenericsRepositories;



namespace Infraestructure.Repositories
{
    public class PrivateMessageRepository : Repository<PrivateMessage>, IPrivateMessageRepository
    {
        public PrivateMessageRepository(AppDbContext context) : base(context) { }
    }
}
