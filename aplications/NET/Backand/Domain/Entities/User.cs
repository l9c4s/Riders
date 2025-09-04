using Domain.Enum;

namespace Domain.Entities
{
    public class User
    {
        public Guid Id { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;

        public AccessLevel AccessLevel { get; set; } = AccessLevel.CommonUser;
        public ICollection<Post> Posts { get; set; } = new List<Post>();
        public ICollection<Comment> Comments { get; set; } = new List<Comment>();

   

        // Friendships where this user is the requester
        public ICollection<Friendship> FriendRequestsSent { get; set; } = new List<Friendship>();
        // Friendships where this user is the addressee
        public ICollection<Friendship> FriendRequestsReceived { get; set; } = new List<Friendship>();

        public Garage? Garage { get; set; }

        public ICollection<UserLocation> Locations { get; set; } = new List<UserLocation>();
    }
}
