using System;

namespace Aplication.Contracts;

public interface IPresenceTracker
{
    Task<bool> UserConnected(string userId, string connectionId);
    Task<bool> UserDisconnected(string userId, string connectionId);

    Task<bool> IsUserOnline(string userId);
    Task<string[]> GetOnlineUsers();
}
