using System;
using System.Threading.Tasks;
using Aplication.Contracts;
using Infraestructure.ExternalServices.Rabbit;
using Moq;
using Xunit;
using Xunit.Sdk;

namespace Test_Riders.ExternalServicesTests;

[Collection("ExternalServices")]
public class RabbitMQServiceIntegrationTests
{
    private readonly RabbitMQService _sut;

    public RabbitMQServiceIntegrationTests(ExternalServicesFixture fixture)
    {
        if (fixture.RabbitMQConnection is null)
            throw new XunitException("RabbitMQ não configurado. Teste ignorado.");

        var presenceTrackerMock = new Mock<IPresenceTracker>(MockBehavior.Loose);

        _sut = new RabbitMQService(fixture.RabbitMQConnection, presenceTrackerMock.Object);
    }

    [Fact(DisplayName = "RabbitMQ: Publish/StartConsuming via fila do usuário envia e recebe mensagem (instância real)")]
    public async Task PublishAndConsume_ShouldSendAndReceive_OnRealRabbitMQ_UserQueue()
    {
        var userId = $"it-user-{Guid.NewGuid()}";
        var expectedMessage = $"Hello from integration test {DateTime.UtcNow.Ticks}";
        string? receivedMessage = null;
        var receivedTcs = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);

        await _sut.StartConsuming(userId, msg =>
        {
            receivedMessage = msg;
            receivedTcs.TrySetResult(true);
        });

        try
        {
            await Task.Delay(150); // pequeno tempo para estabilizar o consumidor
            await _sut.PublishToUserQueue(userId, expectedMessage);

            var completed = await Task.WhenAny(receivedTcs.Task, Task.Delay(5000));
            Assert.True(completed == receivedTcs.Task, "Timeout: mensagem não recebida.");
            Assert.Equal(expectedMessage, receivedMessage);
        }
        finally
        {
            await _sut.StopConsuming(userId);
        }
    }
}