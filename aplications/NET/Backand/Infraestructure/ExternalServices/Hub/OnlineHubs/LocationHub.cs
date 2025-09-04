
using System.Security.Claims;
using System.Text.Json;
using Aplication.Contracts;
using Aplication.Contracts.Hub;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace Infraestructure.Hubs.Config;

[Authorize]
public class LocationHub : Hub
{
    private readonly IPresenceTracker _tracker;
    private readonly IRabbitMqService _rabbitMqService;
    private readonly IHubContext<LocationHub> _hubContext;

    public LocationHub(IPresenceTracker tracker, IRabbitMqService rabbitMqService, IHubContext<LocationHub> hubContext)
    {
        _hubContext = hubContext;
        _tracker = tracker;
        _rabbitMqService = rabbitMqService;
    }


    /// <summary>
    /// Chamado quando um novo cliente se conecta.
    /// A lógica principal de permissão e inscrição acontece aqui.
    /// </summary>
    public override async Task OnConnectedAsync()
    {
        var observerId = Context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(observerId))
        {
            Context.Abort();
            return;
        }
       await _tracker.UserConnected(observerId, Context.ConnectionId);

        var connectionId = Context.ConnectionId;

        Action<string> handleMessage = async (message) =>
        {
            Object locationUpdate = JsonSerializer.Deserialize<object>(message);
            await _hubContext.Clients.Client(connectionId).SendAsync("ReceiveLocationUpdate", locationUpdate);
            await Task.Delay(10000);
        };

        // Passe a ação de callback, agora limpa, para o serviço.
        await _rabbitMqService.StartConsuming(observerId, handleMessage);

        await base.OnConnectedAsync();
    }

    /// <summary>
    /// Chamado quando um cliente se desconecta.
    /// Essencial para limpar as inscrições no RabbitMQ e evitar consumo desnecessário de recursos.
    /// </summary>
    public override async Task OnDisconnectedAsync(Exception exception)
    {
        var observerId = Context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!string.IsNullOrEmpty(observerId))
        {
            _rabbitMqService.StopConsuming(observerId);
            _tracker.UserDisconnected(observerId, Context.ConnectionId);
            
        }
        await base.OnDisconnectedAsync(exception);
    }
}
