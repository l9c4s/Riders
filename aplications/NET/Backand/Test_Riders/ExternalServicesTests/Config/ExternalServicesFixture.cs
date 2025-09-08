using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;
using RabbitMQ.Client.Exceptions;
using StackExchange.Redis;
using Xunit;

namespace Test_Riders.ExternalServicesTests;

public class ExternalServicesFixture : IAsyncLifetime
{
    public IConfiguration Configuration { get; private set; } = default!;
    public IConnection? RabbitMQConnection { get; private set; }
    public IConnectionMultiplexer? RedisConnection { get; private set; }
    public string? PostgresConnectionString { get; private set; }

    public async Task InitializeAsync()
    {
        // BaseDirectory aponta para bin/<config>/<tfm>
        Configuration = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: false)
            .AddJsonFile("appsettings.testing.json", optional: true, reloadOnChange: false)
            .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT")}.json", optional: true, reloadOnChange: false)
            .AddEnvironmentVariables()
            .Build();

        PostgresConnectionString = Configuration.GetConnectionString("DefaultConnection");

        // Conexão RabbitMQ (opcional, se configurado)
        var host = Configuration["RabbitMQ:HostName"];
        var user = Configuration["RabbitMQ:UserName"];
        var pass = Configuration["RabbitMQ:Password"];

        if (!string.IsNullOrWhiteSpace(host))
        {
            var rabbitFactory = new ConnectionFactory
            {
                HostName = host,
                UserName = user,
                Password = pass
            };

            var retries = 5;
            while (retries > 0)
            {
                try
                {
                    RabbitMQConnection = await rabbitFactory.CreateConnectionAsync();
                    if (RabbitMQConnection.IsOpen) break;
                }
                catch (BrokerUnreachableException)
                {
                    retries--;
                    if (retries == 0) throw;
                    await Task.Delay(3000);
                }
            }
        }

        // Conexão Redis (opcional, se configurada)
        var redisCs = Configuration.GetConnectionString("RedisConnection");
        if (!string.IsNullOrWhiteSpace(redisCs))
        {
            RedisConnection = await ConnectionMultiplexer.ConnectAsync(redisCs);
        }
    }

    public async Task DisposeAsync()
    {
        if (RabbitMQConnection is not null)
        {
            await RabbitMQConnection.CloseAsync();
            RabbitMQConnection.Dispose();
        }

        if (RedisConnection is not null)
        {
            await RedisConnection.CloseAsync();
            RedisConnection.Dispose();
        }
    }
}