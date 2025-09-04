using Domain.Contracts;
using Infraestructure.Repositories;


namespace Riders.Config.Injections
{
    public static class RepositoryesInjectionsConfig
    {
        public static IServiceCollection AddRepositoryesInjections(this IServiceCollection services)
        {
            // Repositories
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IPostRepository, PostRepository>();
            services.AddScoped<ICommentRepository, CommentRepository>();
            services.AddScoped<IFriendshipRepository, FriendshipRepository>();
            services.AddScoped<IGarageRepository, GarageRepository>();
            services.AddScoped<IVehicleRepository, VehicleRepository>();
            services.AddScoped<IUserLocationRepository, UserLocationRepository>();
            services.AddScoped<ILikeRepository, LikeRepository>();
            services.AddScoped<IGroupRepository, GroupRepository>();
            services.AddScoped<IUserGroupRepository, UserGroupRepository>();
            services.AddScoped<IPrivateMessageRepository, PrivateMessageRepository>();
            services.AddScoped<IEventRepository, EventRepository>();
            services.AddScoped<IUserEventRepository, UserEventRepository>();
            services.AddScoped<ITokenRepository, TokenRepository>();
            services.AddScoped<ILocationShareRepository, LocationShareRepository>();
            return services;
        }

    }
}
