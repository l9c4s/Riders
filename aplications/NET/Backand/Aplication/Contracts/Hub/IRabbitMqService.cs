using System;

namespace Aplication.Contracts.Hub;

public interface IRabbitMqService
{
        Task PublishToUserQueue(string userId, string message);
        Task StartConsuming(string userId, Action<string> onMessageReceived);
        Task StopConsuming(string userId);
}
