using Domain.Contracts;
using Domain.Entities;
using Infraestructure.Data;
using Infraestructure.Repositories.GenericsRepositories;




namespace Infraestructure.Repositories
{
    public class VehicleRepository : Repository<Vehicle>, IVehicleRepository
    {
        public VehicleRepository(AppDbContext context) : base(context) { }
    }
}
