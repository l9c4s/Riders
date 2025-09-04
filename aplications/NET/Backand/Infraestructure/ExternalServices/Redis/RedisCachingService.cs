using Aplication.Contracts;
using StackExchange.Redis;
using System;
using System.Text.Json;
using System.Threading.Tasks;

namespace Infraestructure.ExternalServices.Implements;

public class RedisCachingService : ICachingService
{
    private readonly IDatabase _database;

    public RedisCachingService(IConnectionMultiplexer redis)
    {
        _database = redis.GetDatabase();
    }

    public async Task<T> GetAsync<T>(string key)
    {
        var value = await _database.StringGetAsync(key);
        if (value.IsNullOrEmpty)
        {
            return default;
        }
        return JsonSerializer.Deserialize<T>(value);
    }

    public async Task RemoveAsync(string key)
    {
        await _database.KeyDeleteAsync(key);
    }

    public async Task SetAsync<T>(string key, T value, TimeSpan? expiry = null)
    {
        var serializedValue = JsonSerializer.Serialize(value);
        await _database.StringSetAsync(key, serializedValue, expiry);
    }
}
