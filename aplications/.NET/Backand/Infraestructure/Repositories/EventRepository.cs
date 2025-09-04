using Domain.Contracts;
using Domain.Entities;
using Infraestructure.Data;
using Infraestructure.Repositories.GenericsRepositories;
using Microsoft.EntityFrameworkCore;



namespace Infraestructure.Repositories
{
    public class EventRepository : Repository<Event>, IEventRepository
    {
        public EventRepository(AppDbContext context) : base(context) { }
    }
}
