using Aplication.Contracts;
using Aplication.Contracts.Hub;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Collections.Concurrent;
using System.Text;

namespace Infraestructure.ExternalServices.Rabbit
{
    public class RabbitMQService : IRabbitMqService, IDisposable
    {

        private IPresenceTracker _presenceTracker;
        private readonly IConnection _connection;
        private readonly ConcurrentDictionary<string, (IChannel Channel, string ConsumerTag)> _consumers = new();
        private const string QueuePrefix = "user_queue_";
        public RabbitMQService(IConnection connection, IPresenceTracker presenceTracker )
        {
            _presenceTracker = presenceTracker;
            _connection = connection;
        }

        public Task PublishToUserQueue(string userId, string message)
        {
            using var channel = _connection.CreateChannelAsync().Result;
            var queueName = $"{QueuePrefix}{userId}";
            var body = Encoding.UTF8.GetBytes(message);
            channel.BasicPublishAsync(exchange: "",
                                 routingKey: queueName,
                                 body: body);

            return Task.CompletedTask;
        }

        public async Task StartConsuming(string userId, Action<string> onMessageReceived)
        {
         


            var channel = await _connection.CreateChannelAsync();
            var queueName = $"{QueuePrefix}{userId}";


             channel =  _connection.CreateChannelAsync().Result;
             await channel.QueueDeclareAsync(queue: queueName,
                                 durable: false,
                                 exclusive: false,
                                 autoDelete: true,
                                 arguments: null);

            var consumer = new AsyncEventingBasicConsumer(channel);

            consumer.ReceivedAsync += async (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                onMessageReceived(message);
                await Task.CompletedTask; 
            };

            var consumerTag = await channel.BasicConsumeAsync(queue: queueName,
                                         autoAck: true,
                                         consumer: consumer);
            _consumers[userId] = (channel, consumerTag);
        }

        public async Task StopConsuming(string userId)
        {
           if (_consumers.TryRemove(userId, out var consumerInfo))
            {
                // Cancela o consumo e fecha o canal específico deste consumidor
                await consumerInfo.Channel.BasicCancelAsync(consumerInfo.ConsumerTag);
                consumerInfo.Channel.CloseAsync();
                consumerInfo.Channel.Dispose();
            }
        }



        

        public void Dispose()
        {
            foreach (var key in _consumers.Keys)
            {
                if (_consumers.TryRemove(key, out var consumerInfo))
                {
                    if(!_presenceTracker.IsUserOnline(key).Result)
                    {
                         consumerInfo.Channel.Dispose();
                    }
                   
                }
            }
        }
    }
}
