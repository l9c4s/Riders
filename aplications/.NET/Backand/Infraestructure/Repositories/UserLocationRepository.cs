using Domain.Contracts;
using Domain.Entities;
using Infraestructure.Data;
using Infraestructure.Repositories.GenericsRepositories;



namespace Infraestructure.Repositories
{
    public class UserLocationRepository : Repository<UserLocation> , IUserLocationRepository
    {
        public UserLocationRepository(AppDbContext context) : base(context) 
        { 

        }
        // Métodos específicos de UserLocation, se necessário
    }
}
