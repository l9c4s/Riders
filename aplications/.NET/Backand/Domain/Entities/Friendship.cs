using Domain.Enum;

namespace Domain.Entities
{
    public class Friendship
    {
        public Guid Id { get; set; }
        public Guid RequesterId { get; set; }
        public User? Requester { get; set; }
        public Guid AddresseeId { get; set; }
        public User? Addressee { get; set; }
        public FriendshipStatus Status { get; set; }
        public DateTime RequestedAt { get; set; }
        public DateTime? RespondedAt { get; set; }
    }
}
