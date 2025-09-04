using System;
using System.Threading.Tasks;
using Infraestructure.ExternalServices.Implements;
using Xunit;
using Xunit.Sdk;


namespace Test_Riders.ExternalServicesTests;

[Collection("ExternalServices")]
public class RedisCachingServiceIntegrationTests
{
    private readonly RedisCachingService _sut;
    private readonly string _testKey = $"test-key:{Guid.NewGuid()}";

    public RedisCachingServiceIntegrationTests(ExternalServicesFixture fixture)
    {
        if (fixture.RedisConnection is null)
            throw new XunitException("Redis não configurado. Teste ignorado.");

        _sut = new RedisCachingService(fixture.RedisConnection);
    }

    [Fact(DisplayName = "Redis: Set/Get deve persistir e recuperar dados (instância real)")]
    public async Task SetAndGet_ShouldPersistAndRetrieveData_FromRealRedis()
    {
        var testObject = new { Name = "Integration Test", Timestamp = DateTime.UtcNow };

        try
        {
            await _sut.SetAsync(_testKey, testObject, TimeSpan.FromSeconds(30));
            var result = await _sut.GetAsync<object>(_testKey);

            Assert.NotNull(result);
        }
        finally
        {
            await _sut.RemoveAsync(_testKey);
        }
    }
}