using System;
using Aplication.Contracts;
using Aplication.Contracts.Hub;
using Infraestructure.ExternalServices.Implements;
using Infraestructure.ExternalServices.Rabbit;
using RabbitMQ.Client;
using StackExchange.Redis;

namespace Riders.Config.ExternalServicesInjectionsConfig;

public static class ExternalServicesInjectionConfig
{
    public static IServiceCollection AddExternalServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Configura a conexão com o RabbitMQ como um Singleton
        services.AddSingleton<IConnection>(sp =>
        {
            var factory = new ConnectionFactory()
            {
                HostName = configuration["RabbitMQ:HostName"],
                UserName = configuration["RabbitMQ:UserName"],
                Password = configuration["RabbitMQ:Password"]
            };
            return factory.CreateConnectionAsync().Result;
        });

        // Configura a conexão com o Redis como um Singleton
        services.AddSingleton<IConnectionMultiplexer>(
                ConnectionMultiplexer.ConnectAsync(configuration.GetConnectionString("RedisConnection")).Result
            );

        // Registra os serviços que usarão essas conexões
        services.AddSingleton<IRabbitMqService, RabbitMQService>();
        services.AddSingleton<ICachingService, RedisCachingService>();

        return services;
    }
}
