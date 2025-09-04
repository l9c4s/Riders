using Aplication.Contracts;

namespace Aplication.Services;

public class PresenceTracker : IPresenceTracker
{
    private static readonly Dictionary<string, List<string>> OnlineUsers =
        new Dictionary<string, List<string>>();

    public Task<bool> UserConnected(string userId, string connectionId)
    {
        bool isOnline = false;
        lock (OnlineUsers)
        {
            if (OnlineUsers.ContainsKey(userId))
            {
                OnlineUsers[userId].Add(connectionId);
            }
            else
            {
                OnlineUsers.Add(userId, new List<string> { connectionId });
                isOnline = true; // O usuário acabou de ficar online
            }
        }

        return Task.FromResult(isOnline);
    }

    public Task<bool> UserDisconnected(string userId, string connectionId)
    {
        bool isOffline = false;
        lock (OnlineUsers)
        {
            if (!OnlineUsers.ContainsKey(userId)) return Task.FromResult(isOffline);

            OnlineUsers[userId].Remove(connectionId);
            if (OnlineUsers[userId].Count == 0)
            {
                OnlineUsers.Remove(userId);
                isOffline = true; // O usuário não tem mais conexões ativas
            }
        }

        return Task.FromResult(isOffline);
    }

    public Task<string[]> GetOnlineUsers()
    {
        string[] onlineUsers;
        lock (OnlineUsers)
        {
            onlineUsers = OnlineUsers.Keys.ToArray();
        }

        return Task.FromResult(onlineUsers);
    }

    public Task<bool> IsUserOnline(string userId)
    {
        bool isOnline;
        lock (OnlineUsers)
        {
            isOnline = OnlineUsers.ContainsKey(userId);
        }

        return Task.FromResult(isOnline);
    }
}
