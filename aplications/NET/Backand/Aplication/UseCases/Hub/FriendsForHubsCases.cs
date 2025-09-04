using System;
using Aplication.Contracts;
using Domain.Contracts;

namespace Aplication.UseCases.Hub;

public class FriendsForHubsCases : IFriendsForHubs
{
   private readonly ILocationShareRepository _locationShareRepository;
    private readonly ICachingService _cachingService;

    public FriendsForHubsCases(ILocationShareRepository locationShareRepository, ICachingService cachingService)
    {
        _locationShareRepository = locationShareRepository;
        _cachingService = cachingService;
    }

    public async Task<IEnumerable<string>> GetTrackableFriendIdsAsync(string observerId)
        {
            var cacheKey = $"user_track_list:{observerId}";
            var trackableIds = await _cachingService.GetAsync<List<string>>(cacheKey);

            if (trackableIds != null)
            {
                return trackableIds;
            }

            // A lógica correta é: "Encontre todas as amizades onde EU (observerId) sou o AddresseeId,
            // o status é Aceito e o compartilhamento de local está ativo".
            // O ID que queremos é o do RequesterId, que é quem está compartilhando a localização.
            var observerGuid = Guid.Parse(observerId);
            var activeShares = await _locationShareRepository.GetActiveSharesForObserverAsync(observerGuid);


            trackableIds = activeShares
                .Select(share => share.SharerId.ToString())
                .ToList();

            await _cachingService.SetAsync(cacheKey, trackableIds, TimeSpan.FromMinutes(5));

            return trackableIds;
        }
}
