namespace Domain.Dto.Friendship
{
    public class RequestFriendshipDto
    {
        public Guid RequesterId { get; set; }
        public Guid AddresseeId { get; set; }
    }
}
