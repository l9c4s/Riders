using Domain.Contracts;
using Domain.Entities;
using Infraestructure.Data;
using Infraestructure.Repositories.GenericsRepositories;





namespace Infraestructure.Repositories
{
    public class UserEventRepository : Repository<UserEvent>, IUserEventRepository
    {
        public UserEventRepository(AppDbContext context) : base(context) { }
    }
}
