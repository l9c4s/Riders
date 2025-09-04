using System;

namespace Aplication.Contracts;

public interface ILocationsUseCases
{
    Task RevokeLocationShareAsync(string sharerId, string observerId);   
    Task GrantLocationShareAsync(string sharerId, string observerId);
    Task UpdateUserLocationAsync(string userId, double latitude, double longitude);
}
