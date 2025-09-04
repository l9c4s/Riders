using System;
using Aplication.Contracts;
using Aplication.Contracts.Hub;
using Aplication.Services;

namespace Riders.Config.Injections.Singleton;

public static class ManageHubInjetions
{

    public static IServiceCollection AddManageHubInjetions(this IServiceCollection services)
    {
        // Add your singleton services here
        services.AddSingleton<IPresenceTracker, PresenceTracker>();

        return services;
    }

}
