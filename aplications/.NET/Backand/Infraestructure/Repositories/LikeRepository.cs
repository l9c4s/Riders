using Domain.Contracts;
using Domain.Entities;
using Infraestructure.Data;
using Infraestructure.Repositories.GenericsRepositories;
using Microsoft.EntityFrameworkCore;



namespace Infraestructure.Repositories
{
    public class LikeRepository : Repository<Like>, ILikeRepository
    {
        public LikeRepository(AppDbContext context) : base(context) { }
    }
}
