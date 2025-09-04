using Domain.Contracts;
using Domain.Entities;
using Infraestructure.Data;
using Infraestructure.Repositories.GenericsRepositories;
using Microsoft.EntityFrameworkCore;


namespace Infraestructure.Repositories
{

    public class GarageRepository : Repository<Garage>, IGarageRepository
    {
        public GarageRepository(AppDbContext context) : base(context) { }
    }
}
