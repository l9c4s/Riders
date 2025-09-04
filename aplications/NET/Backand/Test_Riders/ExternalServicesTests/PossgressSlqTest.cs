using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Npgsql;
using Test_Riders.ExternalServicesTests;
namespace Test_Riders.PossgressSlqTest;

[Collection("ExternalServices")]
public class PossgressSlqTest
{
    private readonly ExternalServicesFixture _fx;

    public PossgressSlqTest(ExternalServicesFixture fx) => _fx = fx;

    [Fact(DisplayName = "Deve conectar no Postgres e executar SELECT 1")]
    public async Task Deve_Conectar_No_Postgres_E_Fazer_Select_1()
    {
        var cs = _fx.PostgresConnectionString ?? _fx.Configuration.GetConnectionString("Postgres");
        Assert.False(string.IsNullOrWhiteSpace(cs), "ConnectionStrings:Postgres não encontrado.");

        await using var conn = new NpgsqlConnection(cs);
        await conn.OpenAsync();

        await using var cmd = new NpgsqlCommand("select 1", conn);
        var result = (int)(await cmd.ExecuteScalarAsync() ?? -1);

        Assert.Equal(1, result);
    }
}