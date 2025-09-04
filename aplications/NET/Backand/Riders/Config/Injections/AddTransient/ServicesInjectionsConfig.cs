using Aplication.Contracts;
using Aplication.Contracts.Hub;
using Aplication.UseCases.Friendship;
using Aplication.UseCases.Hub;
using Aplication.UseCases.Location;
using Aplication.UseCases.Token;
using Aplication.UseCases.User;
using Infraestructure.ExternalServices.Implements;

namespace Riders.Config.Injections
{
    public static class ServicesInjectionsConfig
    {
        public static IServiceCollection AddServicesInjections(this IServiceCollection services)
        {
            services.AddTransient<IUsersUseCases, UserCasesImplements>();
            services.AddTransient<ITokenUseCases, TokenCasesImplements>();
            services.AddTransient<IFriendshipUseCases, FriendshipCasesImplements>();
            services.AddTransient<IFriendsForHubs, FriendsForHubsCases>();
            services.AddTransient<ILocationsUseCases, LocationCases>();
            return services;
        }


    }
}
