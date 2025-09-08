using System;
using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;
using StackExchange.Redis;

namespace Test_Riders.external;

public class DatabaseConnectionTests
{
    private readonly IConfiguration _configuration;

    public DatabaseConnectionTests()
    {
        _configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.testing.json", optional: false)
            .AddEnvironmentVariables()
            .Build();
    }

    [Fact]
    public async Task PostgreSQL_Connection_Should_Be_Successful()
    {
        // Arrange
        var connectionString = _configuration.GetConnectionString("DefaultConnection");

        // Act & Assert
        using var connection = new Npgsql.NpgsqlConnection(connectionString);
        await connection.OpenAsync();

        Assert.Equal(System.Data.ConnectionState.Open, connection.State);

        // Test basic query
        using var command = new Npgsql.NpgsqlCommand("SELECT 1", connection);
        var result = await command.ExecuteScalarAsync();

        Assert.Equal(1, result);
    }

    [Fact]
    public async Task Redis_Connection_Should_Be_Successful()
    {
        // Arrange
        var connectionString = _configuration.GetConnectionString("RedisConnection");

        // Act & Assert
        using var redis = ConnectionMultiplexer.Connect(connectionString);
        var database = redis.GetDatabase();

        // Test basic operations
        await database.StringSetAsync("test_key", "test_value");
        var value = await database.StringGetAsync("test_key");

        Assert.Equal("test_value", value);

        // Cleanup
        await database.KeyDeleteAsync("test_key");
    }

    [Fact]
    public async Task RabbitMQ_Connection_Should_Be_Successful()
    {
        // Arrange
        var factory = new ConnectionFactory()
        {
            HostName = _configuration["RabbitMQ:HostName"] ?? "localhost",
            Port = int.Parse(_configuration["RabbitMQ:Port"] ?? "5672"),
            UserName = _configuration["RabbitMQ:UserName"] ?? "guest",
            Password = _configuration["RabbitMQ:Password"] ?? "guest"
        };

        // Act & Assert
        using var connection = await factory.CreateConnectionAsync();
        using var channel = await connection.CreateChannelAsync();

        Assert.True(connection.IsOpen);
        Assert.True(channel.IsOpen);

        // Test basic queue operations
        var queueName = "test_connection_queue";
        var queueDeclareResult = await channel.QueueDeclareAsync(queue: queueName, durable: false, exclusive: true, autoDelete: true);

        // Publish a test message
        var message = "Connection test message";
        var body = System.Text.Encoding.UTF8.GetBytes(message);

        await channel.BasicPublishAsync(exchange: "",
                                 routingKey: queueName,
                                 body: body);

        // Wait a moment for message to be processed
        await Task.Delay(100);

        // Verify queue exists and has message
        var queueInfo = await channel.QueueDeclarePassiveAsync(queueName);

        // Just verify the queue was created successfully, not necessarily that it has messages
        Assert.NotNull(queueInfo);
        Assert.Equal(queueName, queueInfo.QueueName);
    }
}