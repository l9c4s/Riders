using System;
using System.Text.Json;
using Aplication.Contracts;
using Aplication.Contracts.Hub;
using Aplication.Services;
using Domain.Contracts;
using Domain.Entities;

namespace Aplication.UseCases.Location;

public class LocationCases : ILocationsUseCases
{
    private readonly IRabbitMqService _rabbitMqService;
    private readonly IPresenceTracker _tracker;
    private readonly ILocationShareRepository _locationShareRepository;
    private readonly ICachingService _cachingService; // Adiciona

    public LocationCases(IRabbitMqService rabbitMqService, IPresenceTracker tracker, ILocationShareRepository locationShareRepository, ICachingService cachingService)
    {
        _rabbitMqService = rabbitMqService;
        _tracker = tracker;
        _locationShareRepository = locationShareRepository;
        _cachingService = cachingService;
    }

    public async Task GrantLocationShareAsync(string sharerId, string observerId)
    {
        var sharerGuid = Guid.Parse(sharerId);
        var observerGuid = Guid.Parse(observerId);

        // Evita duplicados: só adiciona se não existir uma permissão.
        var existingShare = await _locationShareRepository.FindShareAsync(sharerGuid, observerGuid);
        if (existingShare == null)
        {
            var newShare = new LocationShare
            {
                SharerId = sharerGuid,
                ObserverId = observerGuid,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };
            await _locationShareRepository.AddAsync(newShare);


            var cacheKey = $"observers:{sharerId}";
            await _cachingService.RemoveAsync(cacheKey);
        }

    }

    public async Task RevokeLocationShareAsync(string sharerId, string observerId)
    {
        var sharerGuid = Guid.Parse(sharerId);
        var observerGuid = Guid.Parse(observerId);

        var existingShare = await _locationShareRepository.FindShareAsync(sharerGuid, observerGuid);
        if (existingShare != null)
        {
            await _locationShareRepository.DeleteAsync(existingShare.Id);
            var cacheKey = $"observers:{sharerId}";
            await _cachingService.RemoveAsync(cacheKey);
        }
    }

    public async Task UpdateUserLocationAsync(string senderUserId, double latitude, double longitude)
    {
        var cacheKey = $"observers:{senderUserId}";

        // 1. Tenta obter a lista de observadores do cache.
        var authorizedObserverIds = await _cachingService.GetAsync<List<string>>(cacheKey);

        // 2. Se não estiver no cache (cache miss), busca no repositório.
        if (authorizedObserverIds == null)
        {
            var authorizedShares = await _locationShareRepository.GetActiveSharesBySharerAsync(Guid.Parse(senderUserId));
            authorizedObserverIds = authorizedShares.Select(s => s.ObserverId.ToString()).ToList();

            // Salva a lista no cache com expiração de 5 minutos.
            await _cachingService.SetAsync(cacheKey, authorizedObserverIds, TimeSpan.FromMinutes(5));
        }

        if (!authorizedObserverIds.Any()) return;

        // O resto da lógica permanece o mesmo...
        var onlineUserIds = await _tracker.GetOnlineUsers();
        var recipients = authorizedObserverIds.Intersect(onlineUserIds).ToList();

        if (!recipients.Any()) return;

        object locationData = new { UserId = senderUserId, Latitude = latitude, Longitude = longitude, Timestamp = DateTime.UtcNow };
        PublishToQueuesAsync(recipients, locationData);

        Console.WriteLine($"Location from user {senderUserId} sent to {recipients.Count()} online observers.");
    }

    private Task PublishToQueuesAsync(IEnumerable<string> userIds, object payload)
    {
        var message = JsonSerializer.Serialize(payload);

        var publishTasks = userIds.Select(userId =>
            _rabbitMqService.PublishToUserQueue(userId, message)
        );

        Task.WhenAll(publishTasks);

        return Task.CompletedTask;
    }


}
