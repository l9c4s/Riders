using System;

namespace Aplication.Contracts;

public interface IFriendsForHubs
{
    Task<IEnumerable<string>> GetTrackableFriendIdsAsync(string observerId);
}
