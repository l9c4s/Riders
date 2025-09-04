using System;
using Infraestructure.Hubs.Config;

namespace Riders.Hubs;

public static class HubsRegistration
{
    public static void MapAllHubs(WebApplication app)
    {
        app.MapHub<LocationHub>("/locationhub");
        // Adicione outros Hubs aqui se necessário
    }
}
